using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
#if UNITY_WEBGL && !UNITY_EDITOR
    public static class EventSourceTracker
    {
        public static Dictionary<string, EventSource> EventSources = new Dictionary<string, EventSource>();
    }
#endif
}
