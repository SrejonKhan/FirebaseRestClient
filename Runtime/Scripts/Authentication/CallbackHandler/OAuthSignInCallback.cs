using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class OAuthSignInCallback
    {
        internal Action<OAuthSignUpResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public OAuthSignInCallback OnSuccess(Action<OAuthSignUpResponse> callback)
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
    }
}
