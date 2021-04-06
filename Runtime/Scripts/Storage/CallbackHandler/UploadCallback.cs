using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseRestClient
{
    public class UploadCallback
    {
        internal Action<UploadResponse> successCallback;
        internal Action<Exception> exceptionCallback;

        public UploadCallback OnSuccess(Action<UploadResponse> callback)
        {
            successCallback += callback;
            return this;
        }

        public UploadCallback OnError(Action<Exception> callback)
        {
            exceptionCallback += callback;
            return this;
        }
    }

    public class UploadResponse
    {
        public string name;
        public string contentType;
        public string timeCreated;
        public string md5Hash;
        public long size;
        public string downloadTokens;
        public string downloadUrl;
    }
}
