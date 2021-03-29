using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{

    public class SignInCallback
    {
        internal Action<SignInResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public SignInCallback OnSuccess(Action<SignInResponse> callback)
        {
            successCallback += callback;
            return this;
        }

        public SignInCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }
    public class SignInResponse
    {
        public string kind;
        public string localId;
        public string email;
        public string displayName;
        public string idToken;
        public bool registered;
        public string refreshToken;
        public int expiresIn;

        public SignInResponse() { }
    }

}