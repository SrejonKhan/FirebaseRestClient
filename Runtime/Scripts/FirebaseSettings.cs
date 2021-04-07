using FullSerializer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FirebaseRestClient
{
    public class FirebaseSettings : ScriptableObject
    {
        [Header("Realtime Database")]
        public string endpoint = "";

        [Header("Authentication")]
        public string webApi = "";
        [Space]

        [Header("Storage")]
        public string storageEndpoint = "";
        [Space]

        [Header("OAuth Providers")]
        public string googleClientId = "";
        public string googleClientSecret = "";
        [Space]
        public string facebookClientId = "";
        public string facebookClientSecret = "";




        private static FirebaseSettings instance;

        public static FirebaseSettings Instance
        {
            get
            {
                LoadSettings();
                return instance;
            }
        }

        public static void LoadSettings()
        {
            if (instance) return;

            instance = FindOrCreateInstance();
            FirebaseConfig.endpoint = instance.endpoint;

            FirebaseConfig.api = instance.webApi;

            FirebaseConfig.googleClientId = instance.googleClientId;
            FirebaseConfig.googleClientSecret = instance.googleClientSecret;

            FirebaseConfig.facebookClientId = instance.facebookClientId;
            FirebaseConfig.facebookClientSecret = instance.facebookClientSecret;


            FirebaseConfig.storageEndpoint = instance.storageEndpoint;
        }

        static FirebaseSettings FindOrCreateInstance()
        {
            FirebaseSettings tempInstance;
            tempInstance = Resources.Load<FirebaseSettings>("FirebaseRestClient");
            tempInstance = tempInstance ? tempInstance : Resources.LoadAll<FirebaseSettings>("").FirstOrDefault();
            tempInstance = tempInstance ? tempInstance : CreateAndSave();
            if (tempInstance == null) throw new System.Exception("Could not create or find setting for Firebase Rest Client.");
            return tempInstance;
        }

        static FirebaseSettings CreateAndSave()
        {
            var tempInstance = CreateInstance<FirebaseSettings>();

#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall = () => SaveAsset(tempInstance);
            }
            else
            {
                SaveAsset(tempInstance);
            }
#endif
            return tempInstance;
        }

        static void SaveAsset(FirebaseSettings instance)
        {
            string dir = "Assets/Resources";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(instance, "Assets/Resources/FirebaseRestClient.asset");
            AssetDatabase.SaveAssets();
#endif
        }

        public static void LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);

            var resData = fsJsonParser.Parse(json);
            object deserializedRes = null;

            fsSerializer serializer = new fsSerializer();
            serializer.TryDeserialize(resData, typeof(Dictionary<string, string>), ref deserializedRes);

            Dictionary<string, string> destructuredRes = (Dictionary<string, string>)deserializedRes;

            instance.endpoint = destructuredRes["endpoint"];
            instance.webApi = destructuredRes["webApi"];
            instance.googleClientId = destructuredRes["googleClientId"];
            instance.googleClientSecret = destructuredRes["googleClientSecret"];
            instance.facebookClientId = destructuredRes["facebookClientId"];
            instance.facebookClientSecret = destructuredRes["facebookClientSecret"];
            instance.storageEndpoint = destructuredRes["storageEndpoint"];
        }

        public static void SaveToJson(string path)
        {
            string json = JsonUtility.ToJson(instance, true);

            File.WriteAllText(path, json);
        }
    }
}
