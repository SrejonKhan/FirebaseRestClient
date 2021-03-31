using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class GetUserCallback 
    {
        internal Action<FirebaseUser> successCallback;
        internal Action<Exception> exceptionCallback;

        public GetUserCallback OnSuccess(Action<FirebaseUser> callback)
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

        public FirebaseUser ToUser(FirebaseUser oldUser)
        {
            oldUser.localId = localId;
            oldUser.email = email;
            oldUser.isEmailVerified = emailVerified;
            oldUser.displayName = displayName;
            oldUser.photoUrl = photoUrl;

            oldUser.validSince = validSince;
            oldUser.lastLoginAt = lastLoginAt;
            oldUser.createdAt = createdAt;
            oldUser.lastRefreshAt = lastRefreshAt ;
            oldUser.disabled = disabled;
            oldUser.customAuth = customAuth;

            return oldUser;
        }
    }
}
