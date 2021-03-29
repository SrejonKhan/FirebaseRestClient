using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class AccessTokenCallback
    {
        internal Action<AccessTokenResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public AccessTokenCallback OnSuccess(Action<AccessTokenResponse> callback)
        {
            successCallback += callback;
            return this;
        }

        public AccessTokenCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

    public class AccessTokenResponse
    {
        public string accessToken;
        public int expiresIn;
        public string tokenType;
        public string refreshToken;
        public string idToken;
        public string userId;
        public string projectId;

        public AccessTokenResponse() { }
    }
}