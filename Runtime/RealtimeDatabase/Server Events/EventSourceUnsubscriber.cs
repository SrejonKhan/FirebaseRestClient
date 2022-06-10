using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class EventSourceUnsubscriber : MonoBehaviour
    {
        private void OnDisable()
        {
            foreach(var es in EventSourceTracker.EventSources)
            {
                es.Value.Close();
            }
        }
    }
}