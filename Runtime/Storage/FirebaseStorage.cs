using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.Networking;
using System;
using FirebaseRestClient.Helper;

namespace FirebaseRestClient
{
    public class FirebaseStorage 
    {
        private string path;
        private UploadCallback callbackHandler;
        private Action<float> uploadProgressCallback;

        public FirebaseStorage() 
        {
            FirebaseSettings.LoadSettings();
        }

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

        public UploadCallback Upload(string filePath, string name)
        {
            callbackHandler = new UploadCallback();

            uploadProgressCallback = null;

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

        public UploadCallback Upload(byte[] data, string name)
        {
            callbackHandler = new UploadCallback();

            uploadProgressCallback = null;

            StaticCoroutine.StartCoroutine(UploadFile(data, name));

            return callbackHandler;
        }



        IEnumerator UploadFile(string filePath, string name)
        {
            UploadHandler uploadHandler = new UploadHandlerFile(filePath);
            //Debug.Log(FirebaseConfig.storageEndpoint + GetAuthParam());
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

            // Authenticate request if any user signed in
            if (FirebaseAuthentication.currentUser != null)
            {
                req.Headers.Add("Authorization", "Bearer " + FirebaseAuthentication.currentUser.accessToken);
            }


            RESTHelper.Post(req, res =>
            {
                UploadResponse uploadResponse = JsonUtility.FromJson<UploadResponse>(res);

                uploadResponse.downloadUrl = $"{FirebaseConfig.storageEndpoint}/{uploadResponse.name.Replace("/","%2F")}?alt=media&token={uploadResponse.downloadTokens}";

                callbackHandler.successCallback?.Invoke(uploadResponse);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            float lastProgress = 0f;            
            //Progress Callback
            if (uploadProgressCallback != null)
            {
                while (uploadHandler.progress != 1)
                {
                    uploadProgressCallback.Invoke(uploadHandler.progress);
                    yield return new WaitUntil(() => 
                    uploadHandler.progress >= 0.95f? uploadHandler.progress == 1f : //If reached 95% or avobe, we will wait for 100% 
                    uploadHandler.progress >= lastProgress + 0.05f); //Else, we will wait for reaching next 5%

                }                
                uploadProgressCallback.Invoke(1f);
                uploadHandler.Dispose();
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

            // Authenticate request if any user signed in
            if (FirebaseAuthentication.currentUser != null)
            {
                req.Headers.Add("Authorization", "Bearer " + FirebaseAuthentication.currentUser.accessToken);
            }

            RESTHelper.Post(req, res =>
            {
                UploadResponse uploadResponse = JsonUtility.FromJson<UploadResponse>(res);

                uploadResponse.downloadUrl = $"{FirebaseConfig.storageEndpoint}/{uploadResponse.name.Replace("/", "%2F")}?alt=media&token={uploadResponse.downloadTokens}";

                callbackHandler.successCallback?.Invoke(uploadResponse);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            float lastProgress = 0f;
            //Progress Callback
            if (uploadProgressCallback != null)
            {
                while (uploadHandler.progress != 1)
                {
                    uploadProgressCallback.Invoke(uploadHandler.progress);
                    yield return new WaitUntil(() =>
                    uploadHandler.progress >= 0.95f ? uploadHandler.progress == 1f : //If reached 95% or avobe, we will wait for 100% 
                    uploadHandler.progress >= lastProgress + 0.05f); //Else, we will wait for reaching next 5%
                }
                uploadProgressCallback.Invoke(1f);
                uploadHandler.Dispose();
            }
        }
    }
}