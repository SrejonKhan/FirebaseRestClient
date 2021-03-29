using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class ChangeEmailCallback
    {
        internal Action<ChangeEmailResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public ChangeEmailCallback OnSuccess(Action<ChangeEmailResponse> callback)
        {
            successCallback += callback;
            return this;
        }

        public ChangeEmailCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

    public class ChangeEmailResponse
    {
        public string localId;
        public string email;
        public string idToken;
        public string refreshToken;
        public bool emailVerified;
    }
}
