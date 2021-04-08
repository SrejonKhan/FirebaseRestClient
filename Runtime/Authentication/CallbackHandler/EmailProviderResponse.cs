using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient 
{
    public class EmailProviderResponse
    {
        public string[] allProviders;
        public bool registered;
        public string sessionId;
        public string[] signinMethods;
    }
}