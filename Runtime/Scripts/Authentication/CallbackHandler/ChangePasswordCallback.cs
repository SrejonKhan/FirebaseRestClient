using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class ChangePasswordCallback
    {
        internal Action<ChangePasswordResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public ChangePasswordCallback OnSuccess(Action<ChangePasswordResponse> callback)
        {
            successCallback += callback;
            return this;
        }

        public ChangePasswordCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }
    public class ChangePasswordResponse
    {
        public string localId;
        public string email;
        public string passwordHash;
        public string idToken;
        public string refreshToken;
    }
}
