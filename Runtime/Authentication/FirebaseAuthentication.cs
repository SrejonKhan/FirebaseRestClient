using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace FirebaseRestClient
{
    public class FirebaseAuthentication
    {
        //private delegate void StateChangedDelegate();
        private event EventHandler onStateChanged;
        
        //State Change 
        public event EventHandler StateChanged 
        {
            add { onStateChanged += value; }
            remove { onStateChanged -= value; }
        }

        private static FirebaseUser currentUser;

        public FirebaseUser CurrentUser { get => currentUser; }

        public FirebaseAuthentication() 
        {
            FirebaseSettings.LoadSettings();
        }

        internal void UpdateCurrentUser() 
        {
            //We will check for if current user exist in persistant store     
        }

        public SignInCallback SignInWithEmailAndPassword(string email, string password)
        {
            SignInCallback callbackHandler = new SignInCallback();

            string rawBody = "{" +
            $"\"email\":\"{email}\"," + //email
            $"\"password\":\"{password}\"," + //password
            $"\"returnSecureToken\":\"true\"" + //Whether or not to return an ID and refresh token. Should always be true.
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post<SignInResponse>(req, res =>
            {
                var user = res.ToUser();
                user.provider = "password";

                currentUser = user;

                onStateChanged?.Invoke(this, null); //Invoke On State Change Event

                Debug.Log(user.accessToken);

                callbackHandler.successCallback?.Invoke(user);

            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public SignInCallback SignInAnonymously()
        {
            SignInCallback callbackHandler = new SignInCallback();

            string rawBody = "{" +
            $"\"returnSecureToken\":\"true\"" + //Whether or not to return an ID and refresh token. Should always be true.
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:signUp",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post<SignInResponse>(req, res =>
            {
                var user = res.ToUser();
                user.provider = "anonymous";
                user.isAnonymous = true;

                currentUser = user;

                onStateChanged.Invoke(this, null); //Invoke On State Change Event

                callbackHandler.successCallback?.Invoke(user);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public OAuthSignInCallback SignInWithOAuth(string token, string providerID)
        {
            OAuthSignInCallback callbackHandler = new OAuthSignInCallback();

            string rawBody = "{" +
            $"\"postBody\":\"access_token={token}&providerId={providerID}\"," + //post body (access token and provider id)
            $"\"requestUri\":\"http://localhost\"," + //URI to which the IDP redirects the user back.
            $"\"returnIdpCredential\":\"true\"," + //Whether to force the return of the OAuth credential on the following errors: FEDERATED_USER_ID_ALREADY_LINKED and EMAIL_EXISTS.
            $"\"returnSecureToken\":\"true\"" + //Whether or not to return an ID and refresh token. Should always be true.
            "}";

            Debug.Log(rawBody);

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post<OAuthSignUpResponse>(req, res =>
            {
                var user = res.ToUser();
                //user.provider = providerID;

                currentUser = user;
                onStateChanged?.Invoke(this, null); //Invoke On State Change Event

                callbackHandler.successCallback?.Invoke(user);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public OAuthSignInCallback SignInWithOAuth(string token, string providerID, OAuthSignInCallback callback)
        {
            OAuthSignInCallback callbackHandler = callback;

            string rawBody = "{" +
            $"\"postBody\":\"access_token={token}&providerId={providerID}\"," + //post body (access token and provider id)
            $"\"requestUri\":\"http://localhost\"," + //URI to which the IDP redirects the user back.
            $"\"returnIdpCredential\":\"true\"," + //Whether to force the return of the OAuth credential on the following errors: FEDERATED_USER_ID_ALREADY_LINKED and EMAIL_EXISTS.
            $"\"returnSecureToken\":\"true\"" + //Whether or not to return an ID and refresh token. Should always be true.
            "}";

            Debug.Log(rawBody);

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post<OAuthSignUpResponse>(req, res =>
            {
                var user = res.ToUser();
                user.provider = providerID;

                currentUser = user;
                onStateChanged?.Invoke(this, null); //Invoke On State Change Event

                callbackHandler.successCallback?.Invoke(user);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public SignUpCallback CreateUserWithEmailAndPassword(string email, string password)
        {
            SignUpCallback callbackHandler = new SignUpCallback();

            string rawBody = "{" +
                $"\"email\":\"{email}\"," + //email
                $"\"password\":\"{password}\"," + //password
                $"\"returnSecureToken\":\"true\"" + //Whether or not to return an ID and refresh token. Should always be true.
                "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:signUp",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post<SignUpResponse>(req, res =>
            {
                var user = res.ToUser();
                user.provider = "password";

                currentUser = user;
                onStateChanged?.Invoke(this, null); //Invoke On State Change Event

                callbackHandler.successCallback?.Invoke(user);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public OAuthSignInCallback GoogleSignIn(string authCode)
        {
            OAuthSignInCallback callbackHandler = new OAuthSignInCallback();

            string url = $"https://oauth2.googleapis.com/token?" +
                $"code={authCode}&" +
                $"client_id={FirebaseConfig.googleClientId}&" +
                $"client_secret={FirebaseConfig.googleClientSecret}&" +
                $"redirect_uri=http://127.0.0.1:5050&" +
                $"grant_type=authorization_code";

            RESTHelper.Post(url, res =>
            {
                var deserializedObj = JsonUtility.FromJson<GoogleIdTokenResponse>(res);
                SignInWithOAuth(deserializedObj.access_token, "google.com", callbackHandler);
            },
            error =>
            {
                string errorText = "";
                RequestErrorHelper.ToDictionary(error).ToList().ForEach(x => errorText += x.Key + " - " + x.Value + "\n");
                Debug.LogError(errorText);

                var err = error as RequestException;
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public OAuthSignInCallback FacebookSignIn(string authCode)
        {
            OAuthSignInCallback callbackHandler = new OAuthSignInCallback();


            string url = $"https://graph.facebook.com/v10.0/oauth/access_token?" +
                $"client_id={FirebaseConfig.facebookClientId}&" +
                $"redirect_uri=http://localhost:5050/&" +
                $"client_secret={FirebaseConfig.facebookClientSecret}&" +
                $"code={authCode}";


            RESTHelper.Get(url, res =>
            {
                Debug.Log(res);
                var deserializedObj = JsonUtility.FromJson<GoogleIdTokenResponse>(res);
                Debug.Log(deserializedObj.access_token);
                SignInWithOAuth(deserializedObj.access_token, "facebook.com", callbackHandler);
            },
            error =>
            {
                string errorText = "";
                RequestErrorHelper.ToDictionary(error).ToList().ForEach(x => errorText += x.Key + " - " + x.Value + "\n");
                Debug.LogError(errorText);

                var err = error as RequestException;
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public GeneralCallback SendEmailVerification()
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            if (currentUser == null)
            {
                return callbackHandler;
            }

            string rawBody = "{" +
            $"\"requestType\":\"VERIFY_EMAIL\"," +
            $"\"idToken\":\"{currentUser.accessToken}\"" + //user email
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post(req, result =>
            {
                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });
            return callbackHandler;
        }

        public GeneralCallback SendPasswordResetEmail(string email)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string rawBody = "{" +
            $"\"requestType\":\"PASSWORD_RESET\"," +
            $"\"email\":\"{email}\"" + //user email
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post(req, result =>
            {
                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });
            return callbackHandler;
        }

        public ObjectCallback<EmailProviderResponse> FetchProvidersForEmail(string email)
        {
            ObjectCallback<EmailProviderResponse> callbackHandler = new ObjectCallback<EmailProviderResponse>();

            string rawBody = "{" +
            $"\"identifier\":\"{email}\"," +
            $"\"continueUri\":\"http://localhost:5050\"" + //user email
            "}";

            RequestHelper req = new RequestHelper
            {
                Uri = "https://identitytoolkit.googleapis.com/v1/accounts:createAuthUri",
                Params = new Dictionary<string, string>
                    {
                        {"key",  FirebaseConfig.api}
                    },
                BodyString = rawBody
            };

            RESTHelper.Post(req, result =>
            {
                EmailProviderResponse emailProviderResponse = JsonUtility.FromJson<EmailProviderResponse>(result);

                callbackHandler.successCallback?.Invoke(emailProviderResponse);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;

        }

        public void SignOut() 
        {
            currentUser = null;
            onStateChanged?.Invoke(this, null); //Invoke On State Change Event
        }
        
    }

    public class ProviderUserInfo
    {
        public string providerId;
        public string federatedId;
        public string email;
        public string rawId;
    }
}
