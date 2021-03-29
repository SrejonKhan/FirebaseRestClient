using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class DictionaryCallback<T1, T2>
    {
        internal Action<Dictionary<T1, T2>> successCallback;
        internal Action<Exception> exceptionCallback;

        public DictionaryCallback<T1, T2> OnSuccess(Action<Dictionary<T1, T2>> callback)
        {
            successCallback += callback;
            return this;
        }

        public DictionaryCallback<T1, T2> OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

}