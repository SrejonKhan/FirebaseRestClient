using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class FirebaseConfig
    {
        //Firebase endpoint
        public static string endpoint = "https://unityrestfirebase-default-rtdb.firebaseio.com";
        
        //Authentication
        public static string api = "AIzaSyATS__kbK_Wtxg2u_lT4kqtTU5bMsJfGbY";
        public static string authEndpoint = "https://identitytoolkit.googleapis.com/v1/accounts";

        public static string googleClientId = "226705604774-3niu3pa5b3p0dj3bb8u4c82g7kangsrf.apps.googleusercontent.com";
        public static string googleClientSecret = "LKKqLfvPAKpORDbrj_IAwfFv";

        public static string facebookClientId = "179400513902699";
        public static string facebookClientSecret = "a47a8db309a4cc5c3cda1f3ec04bc1d4";

        public static string storageEndpoint = "https://firebasestorage.googleapis.com/v0/b/unityrestfirebase.appspot.com/o";
    }
}
