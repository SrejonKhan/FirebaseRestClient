using FirebaseRestClient.Helper;
using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class FirebaseUser
    {
        internal string displayName;
        public string DisplayName { get => displayName; }

        internal string email;
        public string Email { get => email; }

        internal bool isAnonymous = false;
        public bool IsAnonymous { get => isAnonymous; }

        internal bool isEmailVerified = true;
        public bool IsEmailVerified { get => isEmailVerified; }

        internal string photoUrl;
        public string PhotoUrl { get => photoUrl; }


        internal string provider;
        public string Provider { get => provider; }

        internal string localId;
        public string LocalId { get => localId; }
        
        internal string refreshToken;

        internal string accessToken;

        internal string validSince;
        internal string lastLoginAt;
        internal string createdAt;
        internal string lastRefreshAt;
        internal bool disabled;
        internal bool customAuth;

        //Only for internal assembly itself
        internal FirebaseUser()
        {

        }

        internal FirebaseUser(string email, string photoUrl, string accessToken, string refreshToken)
        {
            this.email = email;
            this.photoUrl = photoUrl;
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
        }

        public void Delete()
        {

        }

        public GetUserCallback Reload()
        {
            GetUserCallback callbackHandler = new GetUserCallback();

            string rawBody = "{" +
            $"\"idToken\":\"{accessToken}\"," + //user id token
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:lookup",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post(req, result =>
            {
                Debug.Log(result);
                var resData = fsJsonParser.Parse(result); //in JSON
                object deserializedRes = null;
                //callbackHandler.successCallback?.Invoke();
                fsSerializer serializer = new fsSerializer();
                serializer.TryDeserialize(resData, typeof(Dictionary<string, GetUserResponse>), ref deserializedRes);

                Dictionary<string, GetUserResponse[]> destructuredRes = (Dictionary<string, GetUserResponse[]>)deserializedRes;

                //var userData = JsonHelper.ArrayFromJson<GetUserResponse>(destructuredRes["users"])[0];
                var userData = destructuredRes["users"];



                //callbackHandler.successCallback?.Invoke(userData[0]);                    
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public GeneralCallback ChangeEmail(string newEmail)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string rawBody = "{" +
            $"\"idToken\":\"{accessToken}\"," +
            $"\"email\":\"{newEmail}\"," + //user email
            $"\"returnSecureToken\":\"true\"" + //user email
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:update",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post<ChangeEmailResponse>(req, result =>
            {
                accessToken = string.IsNullOrEmpty(result.idToken)? accessToken : result.idToken;
                refreshToken = string.IsNullOrEmpty(result.refreshToken) ? refreshToken : result.refreshToken;

                FirebaseAuthentication.currentUser = this;

                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });
            return callbackHandler;
        }

        public GeneralCallback ChangePassword(string newPassword)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string rawBody = "{" +
            $"\"idToken\":\"{accessToken}\"," +
            $"\"password\":\"{newPassword}\"," + //user email
            $"\"returnSecureToken\":\"true\"" + //user email
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:update",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post<ChangePasswordResponse>(req, result =>
            {
                accessToken = string.IsNullOrEmpty(result.idToken) ? accessToken : result.idToken;
                refreshToken = string.IsNullOrEmpty(result.refreshToken) ? refreshToken : result.refreshToken;

                FirebaseAuthentication.currentUser = this;

                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public UpdateUserCallback UpdateProfile(string displayName, string photoUrl)
        {
            UpdateUserCallback callbackHandler = new UpdateUserCallback();

            string rawBody = "{" +
            $"\"idToken\":\"{accessToken}\"," +
            $"\"displayName\":\"{displayName}\"," + //user display name
            $"\"photoUrl\":\"{photoUrl}\"," + //user email
            $"\"returnSecureToken\":\"true\"" + 
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:update",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };


            RESTHelper.Post<UpdateProfileResponse>(req, result =>
            {
                this.displayName = string.IsNullOrEmpty(result.displayName) ? this.displayName : result.displayName;
                this.photoUrl = string.IsNullOrEmpty(result.photoUrl) ? this.photoUrl : result.photoUrl;

                FirebaseAuthentication.currentUser = this;

                callbackHandler.successCallback?.Invoke(this);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });
            return callbackHandler;
        }



        public AccessTokenCallback RefreshAccessToken()
        {
            AccessTokenCallback callbackHandler = new AccessTokenCallback();

            string rawBody = "{" +
            $"\"grant_type\":\"refresh_token\"," +
            $"\"refresh_token\":\"{refreshToken}\"" + //refresh token
            "}";


            RequestHelper req = new RequestHelper
            {
                Uri = "https://securetoken.googleapis.com/v1/token",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post(req, result =>
            {
                var resData = fsJsonParser.Parse(result); //in JSON
                object deserializedRes = null;

                fsSerializer serializer = new fsSerializer();
                serializer.TryDeserialize(resData, typeof(Dictionary<string, string>), ref deserializedRes);

                Dictionary<string, string> destructuredRes = (Dictionary<string, string>)deserializedRes;

                var accessTokenRes = new AccessTokenResponse();
                accessTokenRes.accessToken = destructuredRes["access_token"];
                accessTokenRes.expiresIn = Int32.Parse(destructuredRes["expires_in"]);
                accessTokenRes.tokenType = destructuredRes["token_type"];
                accessTokenRes.refreshToken = destructuredRes["refresh_token"];
                accessTokenRes.idToken = destructuredRes["id_token"];
                accessTokenRes.userId = destructuredRes["user_id"];
                accessTokenRes.projectId = destructuredRes["project_id"];

                callbackHandler.successCallback?.Invoke(accessTokenRes);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }
    }
}
