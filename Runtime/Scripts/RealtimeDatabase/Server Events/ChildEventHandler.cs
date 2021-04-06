using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

namespace FirebaseRestClient
{
    public class ChildEventHandler : DownloadHandlerScript
    {
        private enum ReceivedEventType { Put, Patch, KeepAlive }
        private ReceivedEventType eventType;

        internal enum ChildEventType { ValueChanged, ChildAdded, ChildChanged, ChildRemoved }
        internal ChildEventType childEventType;

        public event Action<ChildEventArgs> OnChildEventReceived;

        private readonly MemoryStream dataStream = new MemoryStream(1024);

        private bool isInitial = true; //if first call

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            foreach (var character in data)
            {
                dataStream.WriteByte(character);
                if (character != '\n') continue;

                var stringByte = new byte[dataStream.Length];
                Array.Copy(dataStream.GetBuffer(), stringByte, dataStream.Length);

                string stringData = Encoding.UTF8.GetString(stringByte);

                //OnChildEventReceived?.Invoke(new ChildEventArgs { data = stringData });
                FilterReceivedData(stringData);

                dataStream.SetLength(0);
            }

            return true;
        }

        void FilterReceivedData(string s)
        {
            if (s.Contains("event"))
            {
                string[] splittedParts = s.Split(':');
                string eventString = splittedParts[1].Remove(0, 1);
                switch (eventString)
                {
                    case "put\n":
                        eventType = ReceivedEventType.Put;
                        break;
                    case "patch\n":
                        eventType = ReceivedEventType.Patch;
                        break;
                    case "keep-alive\n":
                        eventType = ReceivedEventType.KeepAlive;
                        break;
                }
            }

            if (s.Contains("data"))
            {
                if (eventType == ReceivedEventType.KeepAlive) return;
                
                /* THIS PART SHOULD BE IMPROVED WITH BETTER SOLUTION, FOR NOW IT'S WORKING AND DYNAMIC
                 * What are we doing -
                 * 1. Splitting out Path(string) and Data(json)
                 * */
                string jsonData = s.Remove(0, 5);
                string pathString = jsonData.Remove(0, 10).Remove(jsonData.IndexOf("data") - 13, jsonData.Length - jsonData.IndexOf("data") + 3).Remove(0, 1);
                string dataString = jsonData.Remove(jsonData.Length - 2, 2).Remove(0, jsonData.IndexOf("data") + 6);

                if (isInitial)
                {
                    isInitial = false;
                    OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, true));
                    return;
                }               

                switch (eventType)
                {
                    case ReceivedEventType.Put:
                        if (childEventType == ChildEventType.ValueChanged)
                            OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));
                        if(childEventType == ChildEventType.ChildAdded)
                            OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));
                        if(childEventType == ChildEventType.ChildRemoved && dataString == "null")
                            OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));

                        //check data null second split
                        break;
                    case ReceivedEventType.Patch:
                        if (childEventType == ChildEventType.ValueChanged)
                            OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));
                        if (childEventType == ChildEventType.ChildChanged)
                            OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));
                        break;
                    case ReceivedEventType.KeepAlive:
                        break;
                }
            }
            //path okay, data null == remove 
        }
    }
}


public struct ChildEventArgs
{
    public bool isInitial;
    public string path; //relative path
    public string data;

    public ChildEventArgs(string path, string data, bool isInitial)
    {
        this.path = path;
        this.data = data;
        this.isInitial = isInitial;
    }
}