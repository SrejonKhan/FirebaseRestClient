using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class RequestErrorHelper
    {
        public static Dictionary<int, string> ToDictionary(Exception exception)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            RequestException error = exception as RequestException;
            try
            {
                Dictionary<string, ErrorResponse> deserializedError = DeserializerHelper.Deserialize<string, ErrorResponse>(error.Response);
                if (deserializedError.ContainsKey("error"))
                    result.Add(Convert.ToInt32(error.StatusCode), deserializedError["error"].message);
            }
            catch
            {
                Dictionary<string, string> deserializedError = DeserializerHelper.Deserialize<string, string>(error.Response);
                if (deserializedError.ContainsKey("error"))
                    result.Add(Convert.ToInt32(error.StatusCode), deserializedError["error"]);
                else
                    result.Add(Convert.ToInt32(error.StatusCode), error.Message);
            }

            return result;
        }
    }

    [Serializable]
    public class ErrorResponse
    {
        public string code;
        public string message;
    }
}