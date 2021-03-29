using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class FirebaseUser
    {
        private string displayName;
        public string DisplayName { get => displayName; }

        private string email;
        public string Email { get => email; }

        private bool isAnonymous;
        public bool IsAnonymous { get => isAnonymous; }

        private bool isEmailVerified;
        public bool IsEmailVerified { get => isEmailVerified; }

        private string photoUrl;
        public string PhotoUrl { get => photoUrl; }


        private string provider;
        public string Provider { get => provider; }

        private string localId;
        public string LocalId { get => localId; }
        
        private string refreshToken;
        
        public void Delete()
        {

        }

        public GetUserCallback GetUser(string idToken)
        {
            GetUserCallback callbackHandler = new GetUserCallback();

            string rawBody = "{" +
            $"\"idToken\":\"{idToken}\"," + //user id token
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

        public ChangeEmailCallback ChangeEmail(string idToken, string newEmail)
        {
            ChangeEmailCallback callbackHandler = new ChangeEmailCallback();

            string rawBody = "{" +
            $"\"idToken\":\"{idToken}\"," +
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
                callbackHandler.successCallback?.Invoke(result);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });
            return callbackHandler;
        }

        public ChangePasswordCallback ChangePassword(string idToken, string newPassword)
        {
            ChangePasswordCallback callbackHandler = new ChangePasswordCallback();

            string rawBody = "{" +
            $"\"idToken\":\"{idToken}\"," +
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
                callbackHandler.successCallback?.Invoke(result);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public UpdateUserCallback UpdateProfile(string idToken, string displayName, string photoUrl)
        {
            UpdateUserCallback callbackHandler = new UpdateUserCallback();

            string rawBody = "{" +
            $"\"idToken\":\"{idToken}\"," +
            $"\"displayName\":\"{displayName}\"" + //user display name
            $"\"photoUrl\":\"{photoUrl}\"" + //user email
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

            RESTHelper.Post<UpdateProfileResponse>(req, result =>
            {
                callbackHandler.successCallback?.Invoke(result);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });
            return callbackHandler;
        }



        public AccessTokenCallback RefreshAccessToken(string refreshToken)
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
