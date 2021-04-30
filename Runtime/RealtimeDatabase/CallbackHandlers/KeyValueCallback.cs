using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class KeyValueCallback<T1,T2>
    {
        internal Action<KeyValuePair<T1, T2>> successCallback;
        internal Action<Exception> exceptionCallback;


        public KeyValueCallback<T1, T2> OnSuccess(Action<KeyValuePair<T1, T2>> callback)
        {
            successCallback += callback;
            return this;
        }

        public KeyValueCallback<T1, T2> OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }
}
