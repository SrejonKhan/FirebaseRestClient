using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public static class EventSourceTracker
    {
        public static Dictionary<string, EventSource> EventSources = new Dictionary<string, EventSource>();
    }
}
