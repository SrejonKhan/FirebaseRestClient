using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class SignUpCallback 
    {
        internal Action<FirebaseUser> successCallback;
        internal Action<Exception> exceptionCallback;

        public SignUpCallback OnSuccess(Action<FirebaseUser> callback)
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

        public FirebaseUser ToUser()
        {
            var tempUser = new FirebaseUser();
            tempUser.localId = localId;
            tempUser.email = email;
            tempUser.accessToken = idToken;
            tempUser.refreshToken = refreshToken;

            return tempUser;
        }
    }
}
