using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseRestClient
{
#if UNITY_WEBGL && !UNITY_EDITOR
    public class EventSource
    {
        private Guid guid = Guid.NewGuid();

        [DllImport("__Internal")]
        private static extern bool EventSourceIsSupported();
        [DllImport("__Internal")]
        private static extern void EventSourceInit(string guid, string url, EventMessage data);
        [DllImport("__Internal")]
        private static extern void EventSourceClose();

        public delegate void EventMessage(string guid, string eventName, string data);

        public event EventMessage OnEventMessage;

        public void Init(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("EventSource URL cannot be empty.");
                return;
            }

            if (!EventSourceIsSupported())
            {
                Debug.LogError("EventSource is not supported in current browser.");
                return;
            }
            string guidStr = guid.ToString();
            EventSourceTracker.EventSources.Add(guidStr, this);

            EventSourceInit(guidStr, url, EventMessageCallback);
        }

        public void Close()
        {
            EventSourceClose();
            EventSourceTracker.EventSources.Remove(guid.ToString());
        }

        [MonoPInvokeCallback(typeof(EventMessage))]
        private static void EventMessageCallback(string guid, string eventName, string data)
        {
            if (!EventSourceTracker.EventSources.ContainsKey(guid))
            {
                Debug.LogError("EventSource not found! Guid - " + guid);
                return;
            }
            var es = EventSourceTracker.EventSources[guid];
            es.OnEventMessage.Invoke(guid, eventName, data);
        }
    }
#endif
}
