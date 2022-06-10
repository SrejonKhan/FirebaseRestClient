using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using FullSerializer;


namespace FirebaseRestClient
{
    public class ChildEventHandler : DownloadHandlerScript
    {
        private enum ReceivedEventType { Put, Patch, KeepAlive }
        private ReceivedEventType eventType;

        internal enum ChildEventType { ValueChanged, ChildAdded, ChildChanged, ChildRemoved }
        internal ChildEventType childEventType;

        internal bool shallow = false;

        public event Action<ChildEventArgs> OnChildEventReceived;

        private readonly MemoryStream dataStream = new MemoryStream(1024);

        private bool isInitial = true; //if first call

        private List<Dictionary<string, Dictionary<string, fsData>>> nodes = new List<Dictionary<string, Dictionary<string, fsData>>>();


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

        public void ReceiveWebGlData(string id, string eventName, string data)
        {
            switch (eventName)
            {
                case "put":
                    eventType = ReceivedEventType.Put;
                    break;
                case "patch":
                    eventType = ReceivedEventType.Patch;
                    break;
            }
            ProcessDataStr(data);
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
                string jsonData = s.Remove(0, 5);
                ProcessDataStr(jsonData);
            }
        }

        void ProcessDataStr(string jsonData)
        {
            /* TODO THIS PART SHOULD BE IMPROVED WITH BETTER SOLUTION, FOR NOW IT'S WORKING AND DYNAMIC
                 * What are we doing -
                 * 1. Splitting out Path(string) and Data(json)
                 * */
            
            // new
            var jsonDict = fsJsonParser.Parse(jsonData).AsDictionary;
            string pathString = jsonDict["path"].AsString;
            string dataString = jsonDict["data"].ToString();

            // old
            //string pathString = jsonData.Remove(0, 10).
            //    Remove(jsonData.IndexOf("data") - 13, jsonData.Length - jsonData.IndexOf("data") + 3).
            //    Remove(0, 1);
            //string dataString = jsonData.Remove(jsonData.Length - 2, 2).
            //    Remove(0, jsonData.IndexOf("data") + 6);

            if (isInitial)
            {
                isInitial = false;
                OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, true));
                var resData = fsJsonParser.Parse(dataString);
                if (childEventType == ChildEventType.ChildAdded) GetSnapshot(resData, ""); //store snapshot
                return;
            }

            switch (eventType)
            {
                //TODO Shallow method for all events
                case ReceivedEventType.Put:
                    //Value Changed Event
                    if (childEventType == ChildEventType.ValueChanged)
                    {
                        if (shallow && pathString.Split('/').Length > 1) return;

                        OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));
                    }

                    //Child Added Event
                    if (childEventType == ChildEventType.ChildAdded)
                    {
                        if (shallow && pathString.Split('/').Length > 1) return;

                        if (!CheckCachedChild(pathString)) //make sure that it doesn't exist in snapshot
                        {
                            OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));
                            AddToSnapshot(pathString, dataString); //add that child to snapshot
                        }
                    }

                    //Child Removed Event
                    if (childEventType == ChildEventType.ChildRemoved && dataString == "null")
                    {
                        if (shallow && pathString.Split('/').Length > 1) return;

                        OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));
                    }

                    //Child Changed Event
                    if (childEventType == ChildEventType.ChildChanged)
                    {
                        if (shallow && pathString.Split('/').Length > 1) return;

                        OnChildEventReceived?.Invoke(new ChildEventArgs(pathString, dataString, false));
                    }

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

        void GetSnapshot(fsData data, string path, bool isNested = false)
        {
            if (!data.IsDictionary) return;

            var dict = data.AsDictionary;
            foreach (var key in dict.Keys.ToList())
            {
                if (!shallow && dict[key].IsDictionary) //If any nested child available
                {
                    string newPath = String.IsNullOrEmpty(path) ? key : path + "/" + key;
                    GetSnapshot(dict[key], newPath, true);
                }
                dict[key] = new fsData(""); //makes value null for optimization
            }

            //Store in a list, key is node path, value is dictionary of childs, fsData is always empty string
            nodes.Add(new Dictionary<string, Dictionary<string, fsData>> { { path, dict } });
        }

        void AddToSnapshot(string path, string jsonData)
        {
            string[] splittedPath = path.Split('/');
            string parentNode = splittedPath.Length > 1 ? path.Replace($"/{splittedPath[splittedPath.Length - 1]}", "") : "";


            var targetedDict = new Dictionary<string, fsData>();

            foreach (var node in nodes)
            {
                foreach (var child in node.Keys.ToList())
                {
                    if (child == parentNode) targetedDict = node[child];
                }
            }

            if (targetedDict.Keys.ToList().IndexOf(splittedPath[splittedPath.Length - 1]) == -1)
            {
                var fsData = fsJsonParser.Parse(jsonData);

                if (fsData.IsDictionary)
                {
                    GetSnapshot(fsData, path); //store snapshot
                }
                else
                {
                    targetedDict.Add(splittedPath[splittedPath.Length - 1], new fsData(""));
                }
            }
        }

        bool CheckCachedChild(string path)
        {
            // nodes is a list of Dictionary. 
            // Each dictionary = each node from initial path
            //
            // Each dictionary (node) from list has the following pair-
            // Key = path from initial path e.g /users 
            // Value = another dictionary of Childs of the following path (/users)
            //
            // Dictionary of value has following pair -
            // Key = child node name e.g srejonkhan. Remember, it's being concatenated with node key. users + "/" + srejonkhan
            // Value = fsData, contains child data. Eventually turned into a empty string for optimization.
            //    
            // A node get in depth of database to avoid miscommunication with update child event.
            // It's better to listen to shallow child


            foreach (var node in nodes)//Get each node
            {
                foreach (var nestedChildNode in node) //get each child node from node dictionary
                {
                    foreach (var nestedChild in nestedChildNode.Value) //loop through each child
                    {
                        string nodePath = String.IsNullOrEmpty(nestedChildNode.Key) ? nestedChild.Key : nestedChildNode.Key + "/" + nestedChild.Key;
                        if (nodePath == path) return true; //Child exists
                    }
                }
            }
            return false;
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

    public class ServerSentMessage
    {
        public string path;
        public string data;
    }
}


