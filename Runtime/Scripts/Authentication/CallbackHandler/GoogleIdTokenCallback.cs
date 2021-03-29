using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class GoogleIdTokenCallback
    {

    }

    public class GoogleIdTokenResponse 
    {
        public string access_token;
        public string expires_in;
        public string token_type;
        public string scope;
        public string refresh_token;
    }
}
