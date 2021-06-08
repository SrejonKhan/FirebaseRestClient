using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using FirebaseRestClient.Helper;

namespace FirebaseRestClient
{
    public class EventUnsubscriber : MonoBehaviour
    {
        internal RequestHelper requestHelper;
        internal RedirectionListener redirectionListener;

        internal void AddToUnsubscriber(RequestHelper requestHelper)
        {
            this.requestHelper = requestHelper;
        }

        internal void AddToUnsubscriber(RedirectionListener redirectionListener)
        {
            this.redirectionListener = redirectionListener;
        }

        void OnDisable()
        {
            Debug.Log("Unsubscribed all Firebase Events");
            requestHelper?.Abort();
            redirectionListener?.Stop();
        }
    }
}