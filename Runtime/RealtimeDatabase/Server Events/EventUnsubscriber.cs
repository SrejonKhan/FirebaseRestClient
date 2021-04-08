using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;

namespace FirebaseRestClient
{
    public class EventUnsubscriber : MonoBehaviour
    {
        internal RequestHelper requestHelper;
        internal void AddToUnsubscriber(RequestHelper requestHelper)
        {
            this.requestHelper = requestHelper;
        }

        void OnDisable()
        {
            Debug.Log("Unsubscribed all Firebase Events");
            requestHelper?.Abort();
        }
    }
}