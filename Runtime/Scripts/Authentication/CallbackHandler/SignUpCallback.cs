using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class SignUpCallback 
    {
        internal Action<SignUpResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public SignUpCallback OnSuccess(Action<SignUpResponse> callback)
        {
            successCallback += callback;
            return this;
        }

        public SignUpCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

    public class SignUpResponse
    {
        public string kind;
        public string idToken;
        public string email;
        public string refreshToken;
        public int expiresIn;
        public string localId;

        public SignUpResponse() { }
    }
}
