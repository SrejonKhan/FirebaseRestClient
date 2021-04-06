using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.Networking;
using System;

namespace FirebaseRestClient
{
    public class FirebaseStorage 
    {
        private string path;
        private UploadCallback callbackHandler;
        private Action<float> uploadProgressCallback;

        public FirebaseStorage() { }

        internal FirebaseStorage(string path)
        {
            this.path = path;     
        }

        public FirebaseStorage Child(string value)
        {
            string newPath = string.IsNullOrEmpty(path) ? value : $"{path}/{value}";
            return new FirebaseStorage(newPath);
        }

        public UploadCallback Upload(string filePath, string name, Action<float> progressCallback)
        {
            callbackHandler = new UploadCallback();

            uploadProgressCallback = progressCallback;

            StaticCoroutine.StartCoroutine(UploadFile(filePath, name));

            return callbackHandler;
        }

        public UploadCallback Upload(byte[] data, string name, Action<float> progressCallback)
        {
            callbackHandler = new UploadCallback();

            uploadProgressCallback = progressCallback;

            StaticCoroutine.StartCoroutine(UploadFile(data, name));

            return callbackHandler;
        }



        IEnumerator UploadFile(string filePath, string name)
        {
            UploadHandler uploadHandler = new UploadHandlerFile(filePath);
            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.storageEndpoint,
                Params = new Dictionary<string, string>
                {
                    {"uploadType", "media"},
                    { "name", string.IsNullOrEmpty(path)?name:$"{path}/{name}" }
                },
                ContentType = "application/octet-stream",
                UploadHandler = uploadHandler
            };

            RESTHelper.Post(req, res =>
            {
                UploadResponse uploadResponse = JsonUtility.FromJson<UploadResponse>(res);
<<<<<<< HEAD
                uploadResponse.downloadUrl = $"{FirebaseConfig.storageEndpoint}/{uploadResponse.name.Replace("/","%2F")}?alt=media&token={uploadResponse.downloadTokens}";
=======
                uploadResponse.downloadUrl = $"{FirebaseConfig.storageEndpoint}/{uploadResponse.name}?alt=media&token={uploadResponse.downloadTokens}";
>>>>>>> df08b19f821e257b9236010c4cc6ea0143d0b2a8

                callbackHandler.successCallback?.Invoke(uploadResponse);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            //Progress Callback
            if (uploadProgressCallback != null)
            {
                while (uploadHandler.progress != 1)
                {
                    uploadProgressCallback.Invoke(uploadHandler.progress);
                    yield return new WaitForSecondsRealtime(0.25f);
                }
                uploadProgressCallback?.Invoke(1f);
            }
        }

        IEnumerator UploadFile(byte[] data, string name)
        {
            UploadHandler uploadHandler = new UploadHandlerRaw(data);
            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.storageEndpoint,
                Params = new Dictionary<string, string>
                {
                    {"uploadType", "media"},
                    { "name", string.IsNullOrEmpty(path)?name:$"{path}/{name}" }
                },
                ContentType = "application/octet-stream",
                UploadHandler = uploadHandler
            };

            RESTHelper.Post(req, res =>
            {
                UploadResponse uploadResponse = JsonUtility.FromJson<UploadResponse>(res);
<<<<<<< HEAD
                uploadResponse.downloadUrl = $"{FirebaseConfig.storageEndpoint}{uploadResponse.name.Replace("/","%2F")}?alt=media&token={uploadResponse.downloadTokens}";
=======
                uploadResponse.downloadUrl = $"{FirebaseConfig.storageEndpoint}{uploadResponse.name}?alt=media&token={uploadResponse.downloadTokens}";
>>>>>>> df08b19f821e257b9236010c4cc6ea0143d0b2a8

                callbackHandler.successCallback?.Invoke(uploadResponse);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            //Progress Callback
            if (uploadProgressCallback != null)
            {
                while (uploadHandler.progress != 1)
                {
                    uploadProgressCallback.Invoke(uploadHandler.progress);
                    yield return new WaitForSecondsRealtime(0.25f);
                }
                uploadProgressCallback?.Invoke(1f);
            }
        }

    }
}