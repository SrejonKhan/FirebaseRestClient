using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class GetUserCallback 
    {
        internal Action<GetUserResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public GetUserCallback OnSuccess(Action<GetUserResponse> callback)
        {
            successCallback += callback;
            return this;
        }

        public GetUserCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

    public class GetUserResponse
    {
        public string localId;
        public string email;
        public string passwordHash;
        public bool emailVerified;
        public string displayName;
        public string passwordUpdatedAt;
        public ProviderUserInfo[] providerUserInfo;
        public string photoUrl;
        public string validSince;
        public string lastLoginAt;
        public string createdAt;
        public string lastRefreshAt;
        public bool disabled;
        public bool customAuth;
    }
}
