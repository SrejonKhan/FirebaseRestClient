using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class OAuthSignInCallback
    {
        internal Action<FirebaseUser> successCallback;
        internal Action<Exception> exceptionCallback;

        public OAuthSignInCallback OnSuccess(Action<FirebaseUser> callback)
        {
            successCallback += callback;
            return this;
        }

        public OAuthSignInCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

    public class OAuthSignUpResponse
    {
        public string federatedId;
        public string providerId;
        public string email;
        public bool emailVerified;
        public int firstName;
        public string fullName;
        public string lastName;
        public string photoUrl;
        public string localId;
        public string displayName;
        public string idToken;
        public string refreshToken;
        public string expiresIn;
        public string oauthIdToken;
        public string rawUserInfo;

        public OAuthSignUpResponse() { }

        internal FirebaseUser ToUser()
        {
            var tempUser = new FirebaseUser();
            tempUser.localId = localId;
            tempUser.email = email;
            tempUser.isEmailVerified = emailVerified;
            tempUser.displayName = displayName;
            tempUser.photoUrl = photoUrl;
            tempUser.provider = providerId;
            tempUser.accessToken = idToken;
            tempUser.refreshToken = refreshToken;

            return tempUser;
        }
    }
}
