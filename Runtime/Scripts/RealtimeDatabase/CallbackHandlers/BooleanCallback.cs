using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class BooleanCallback
    {
        internal Action<bool> successCallback;
        internal Action<Exception> exceptionCallback;

        public BooleanCallback OnSuccess(Action<bool> callback)
        {
            successCallback += callback;
            return this;
        }

        public BooleanCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }
}
