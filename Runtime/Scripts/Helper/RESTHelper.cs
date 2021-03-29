using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFirebaseREST
{
    public class RESTHelper
    {
        public static void PutJson(RequestHelper request, Action<ResponseHelper> response, Action<Exception> error)
        {
            RestClient.Put(request).Then(res => response(res)).Catch(err => error(err));
        }

        public static void Put<T>(RequestHelper request, Action<T> response, Action<Exception> error)
        {
            RestClient.Put<T>(request).Then(res => response(res)).Catch(err => error(err));
        }

        public static void Post(string url, Action<string> response, Action<Exception> error)
        {
            RestClient.Post(url, null).Then(res => response(res.Text)).Catch(err => error(err));
        }

        public static void Post(RequestHelper request, Action<string> response, Action<Exception> error)
        {
            RestClient.Post(request).Then(res => response(res.Text)).Catch(err => error(err));
        }

        public static void Post<T>(RequestHelper request, Action<T> response, Action<Exception> error)
        {
            RestClient.Post<T>(request).Then(res => response(res)).Catch(err => error(err));
        }

        public static void Patch(RequestHelper request, Action<ResponseHelper> response, Action<Exception> error)
        {
            request.Method = "PATCH";
            RestClient.Request(request).Then(res => response(res)).Catch(err => error(err));
        }

        public static void Get(string url, Action<string> response, Action<Exception> error)
        {
            RestClient.Get(url).Then(res => response(res.Text)).Catch(err => error(err));
        }

        public static void Get(RequestHelper request, Action<Exception> error)
        {
            RestClient.Get(request).Catch(err => error(err));
        }

        public static void Get(RequestHelper request, Action<ResponseHelper> response, Action<Exception> error)
        {
            RestClient.Get(request).Then(res => response(res)).Catch(err => error(err));
        }

        public static void Get<T>(RequestHelper request, Action<T> response, Action<Exception> error)
        {
            RestClient.Get<T>(request).Then(res => response(res)).Catch(err => error(err));
        }

        public static void Get(string route, Action<ResponseHelper> response, Action<Exception> error)
        {
            RestClient.Get(route).Then(res => response(res)).Catch(err => error(err));
        }

        public static void Get<T>(string route, Action<T> response, Action<Exception> error)
        {
            RestClient.Get<T>(route).Then(res => response(res)).Catch(err => error(err));
        }

        public static void GetArray<T>(string route, Action<T[]> response, Action<Exception> error)
        {
            RestClient.GetArray<T>(route).Then(res => response(res)).Catch(err => error(err));
        }

        public static void Delete(string route, Action<ResponseHelper> response, Action<Exception> error)
        {
            RestClient.Delete(route).Then(res => response(res)).Catch(err => error(err));
        }


    }
}
