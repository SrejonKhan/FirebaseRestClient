using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class UpdateUserCallback
    {
        internal Action<UpdateProfileResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public UpdateUserCallback OnSuccess(Action<UpdateProfileResponse> callback)
        {
            successCallback += callback;
            return this;
        }

        public UpdateUserCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }
    public class UpdateProfileResponse
    {
        public string email;
        public string displayName;
        public string photoUrl;
        public bool providerUserInfo;
        public string idToken;
        public string refreshToken;
        public int expiresIn;

        public UpdateProfileResponse() { }
    }
}

