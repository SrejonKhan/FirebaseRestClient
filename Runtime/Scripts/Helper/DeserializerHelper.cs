using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class DeserializerHelper : MonoBehaviour
    {
        public static Dictionary<T1, T2> Deserialize<T1, T2>(string json)
        {
            var fsData = fsJsonParser.Parse(json); //in JSON
            object deserializedRes = null;

            fsSerializer serializer = new fsSerializer();

            serializer.TryDeserialize(fsData, typeof(Dictionary<T1, T2>), ref deserializedRes).AssertSuccess();

            return (Dictionary<T1, T2>)deserializedRes;
        }
    }
}
