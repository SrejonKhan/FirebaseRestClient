using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class StringCallback
    {
        internal Action<string> successCallback;
        internal Action<Exception> exceptionCallback;

        public StringCallback OnSuccess(Action<string> callback)
        {
            successCallback += callback;
            return this;
        }

        public StringCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }

        public StringCallback ToList()
        {
            
            return this;
        }
    }
}