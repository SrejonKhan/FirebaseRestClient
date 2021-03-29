using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
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

        public static string facebookClientId = "766602067617680";
        public static string facebookClientSecret = "f312b4f85f642b2a566ad512bc01bbec";
    }
}
