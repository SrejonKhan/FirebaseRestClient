using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class GeneralCallback
    {
        internal Action successCallback;
        internal Action<Exception> exceptionCallback;

        public GeneralCallback OnSuccess(Action callback)
        {
            successCallback += callback;
            return this;
        }

        public GeneralCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }
}
