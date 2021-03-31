using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class UpdateUserCallback
    {
        internal Action<FirebaseUser> successCallback;
        internal Action<Exception> exceptionCallback;

        public UpdateUserCallback OnSuccess(Action<FirebaseUser> callback)
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
        public string localId;
        public string email;
        public string displayName;
        public string photoUrl;
        public bool providerUserInfo;
        public string idToken;
        public string refreshToken;
        public int expiresIn;

        public UpdateProfileResponse() { }

        public FirebaseUser ToUser(FirebaseUser oldUser)
        {
            oldUser.localId = localId;
            oldUser.email = email;
            oldUser.displayName = displayName;
            oldUser.photoUrl = photoUrl;
            oldUser.accessToken = idToken;
            oldUser.refreshToken = refreshToken;

            return oldUser;
        }
    }
}

