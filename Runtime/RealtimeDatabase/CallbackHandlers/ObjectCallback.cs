using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class ObjectCallback <T>
    {
        internal Action<T> successCallback;
        internal Action<Exception> exceptionCallback;

        internal string hasChildNode;

        public ObjectCallback<T> OnSuccess(Action<T> callback)
        {
            successCallback += callback;
            return this;
        }

        public ObjectCallback<T> OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

    public class ObjectCallback
    {
        internal Action<object> successCallback;
        internal Action<Exception> exceptionCallback;

        internal string hasChildNode;

        public ObjectCallback OnSuccess(Action<object> callback)
        {
            successCallback += callback;
            return this;
        }

        public ObjectCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

}