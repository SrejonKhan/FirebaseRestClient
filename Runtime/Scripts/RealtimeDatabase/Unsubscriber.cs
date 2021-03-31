using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;

namespace FirebaseRestClient
{
    public class Unsubscriber : MonoBehaviour
    {
        internal RequestHelper requestHelper;
        internal void AddToUnsubscriber(RequestHelper requestHelper)
        {
            this.requestHelper = requestHelper;
        }

        void OnDisable()
        {
            Debug.Log("Unsubscribed");
            requestHelper?.Abort();
        }
    }
}