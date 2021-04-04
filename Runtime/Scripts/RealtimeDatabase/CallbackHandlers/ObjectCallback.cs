using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class ObjectCallback <T1>
    {
        internal Action<T1> successCallback;
        internal Action<Exception> exceptionCallback;

        internal string hasChildNode;

        public ObjectCallback<T1> OnSuccess(Action<T1> callback)
        {
            successCallback += callback;
            return this;
        }

        public ObjectCallback<T1> OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

}