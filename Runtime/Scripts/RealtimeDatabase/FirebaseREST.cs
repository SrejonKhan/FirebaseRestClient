using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FirebaseRestClient
{
    public class FirebaseREST
    {
        private string path; /// path from root
        
        //Filters
        private string limitToFirstValue;
        private string limitToLastValue;
        private string startAtValue;
        private string endAtValue;
        private string equalToValue;

        public FirebaseREST() { }

        internal FirebaseREST(string path)
        {
            this.path = path; 
        }


        internal FirebaseREST(string path, FirebaseFilters filters)
        {
            this.path = path;
            FromFirebaseFilters(filters);
        }


        public FirebaseREST Child(string value)
        {
            string newPath = String.IsNullOrEmpty(path) ? value : $"{path}/{value}";
            return new FirebaseREST(newPath, ToFirebaseFilters());
        }


        public GeneralCallback Write<T>(T body)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                Body = body
            };



            RESTHelper.Put<T>(req, res =>
            {
                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public GeneralCallback Write(string key, string value)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string json = "{" +
                $"\"{key}\":\"{value}\"" +
                "}";

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                BodyString = json
            };

            RESTHelper.PutJson(req, res =>
            {
                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }



        public GeneralCallback Write(string json)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                BodyString = json
            };

            RESTHelper.PutJson(req, res =>
            {
                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }


        public StringCallback Push<T>(T body)
        {
            StringCallback callbackHandler = new StringCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                Body = body
            };

            RESTHelper.Post(req, res =>
            {
                var resData = fsJsonParser.Parse(res); //in JSON
                object deserializedRes = null;

                fsSerializer serializer = new fsSerializer();
                serializer.TryDeserialize(resData, typeof(Dictionary<string, string>), ref deserializedRes);

                Dictionary<string, string> destructuredRes = (Dictionary<string, string>)deserializedRes;
                callbackHandler.successCallback?.Invoke(destructuredRes["name"]); //UID
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public GeneralCallback Remove()
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string route = FirebaseConfig.endpoint + "/" + path + ".json";

            RESTHelper.Delete(route, res =>
            {
                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public DictionaryCallback<string,T> Read<T>()
        {
            DictionaryCallback<string, T> callbackHandler = new DictionaryCallback<string, T>();

            string route = FirebaseConfig.endpoint + "/" + path + ".json";

            RESTHelper.Get(route, res =>
            {
                var resData = fsJsonParser.Parse(res.Text); //in JSON

                object deserializedRes = null;

                fsSerializer serializer = new fsSerializer();
                serializer.TryDeserialize(resData, typeof(Dictionary<string, T>), ref deserializedRes);

                Dictionary<string, T> destructuredRes = (Dictionary<string, T>)deserializedRes;
                callbackHandler.successCallback?.Invoke(destructuredRes); //UID
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public StringCallback RawRead()
        {
            StringCallback callbackHandler = new StringCallback();

            string route = FirebaseConfig.endpoint + "/" + path + ".json";

            RESTHelper.Get(route, res =>
            {
                callbackHandler.successCallback?.Invoke(res.Text); //in JSON
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public GeneralCallback Update<T>(T body)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                Body = body
            };
            RESTHelper.Patch(req, res =>
            {
                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public GeneralCallback Update(string json)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                BodyString = json,
            };
            RESTHelper.Patch(req, res =>
            {
                callbackHandler.successCallback?.Invoke();
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public void ValueChanged(Action<ChildEventArgs> callback)
        {
            ChildEventHandler childEventHandler = new ChildEventHandler();
            childEventHandler.OnChildEventReceived += callback;
            childEventHandler.childEventType = ChildEventHandler.ChildEventType.ValueChanged;

            ChildEventListen(childEventHandler);
        }

        public void ChildAdded(Action<ChildEventArgs> callback)
        {
            ChildEventHandler childEventHandler = new ChildEventHandler();
            childEventHandler.OnChildEventReceived += callback;
            childEventHandler.childEventType = ChildEventHandler.ChildEventType.ChildAdded;

            ChildEventListen(childEventHandler);
        }

        public void ChildRemoved(Action<ChildEventArgs> callback)
        {
            ChildEventHandler childEventHandler = new ChildEventHandler();
            childEventHandler.OnChildEventReceived += callback;
            childEventHandler.childEventType = ChildEventHandler.ChildEventType.ChildRemoved;

            ChildEventListen(childEventHandler);
        }

        public void ChildChanged(Action<ChildEventArgs> callback)
        {
            ChildEventHandler childEventHandler = new ChildEventHandler();
            childEventHandler.OnChildEventReceived += callback;
            childEventHandler.childEventType = ChildEventHandler.ChildEventType.ChildChanged;

            ChildEventListen(childEventHandler);
        }


        void ChildEventListen(ChildEventHandler eventHandler)
        {
            RequestHelper req = new RequestHelper
            {
                Headers = new Dictionary<string, string>
                {
                    { "Accept", "text/event-stream"}
                },
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                DownloadHandler = eventHandler,
                Retries = int.MaxValue,
                RetrySecondsDelay = 1
            };

            GameObject go = new GameObject("FirebaseRestUnsubscriber");
            go.AddComponent<Unsubscriber>().requestHelper = req;
            MonoBehaviour.DontDestroyOnLoad(go);

            //Error handling are being handled internally
            RESTHelper.Get(req, err => RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => Debug.Log(x.Key + " - " + x.Value)));
        }

        public StringCallback OrderByChild(string key)
        {
            StringCallback callbackHandler = new StringCallback();

            //Dictionary<string, string> queryParams = GetFilterCollection(false);
            //queryParams.Add("orderBy", "\"" + key + "\"");

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                Params = new Dictionary<string, string>
                        {
                            {"orderBy", "\"" + key + "\""}
                        }
            };

            //adding filters if there is
            GetFilterCollection(false)?.ToList().ForEach(x => req.Params.Add(x.Key, x.Value));

            req.Params.ToList().ForEach(x => Debug.Log(x.Key + " - " + x.Value));


            RESTHelper.Get(req, res =>
            {
                callbackHandler.successCallback?.Invoke(res.Text); //in JSON
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public StringCallback OrderByKey()
        {
            StringCallback callbackHandler = new StringCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                Params = new Dictionary<string, string>
                    {
                        {"orderBy", "\"$key\""}
                    }
            };

            //adding filters if there is
            GetFilterCollection(true)?.ToList().ForEach(x => req.Params.Add(x.Key, x.Value));

            RESTHelper.Get(req, res =>
            {
                callbackHandler.successCallback?.Invoke(res.Text); //in JSON
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        public StringCallback OrderByValue()
        {
            StringCallback callbackHandler = new StringCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + "/" + path + ".json",
                Params = new Dictionary<string, string>
                    {
                        {"orderBy", "\"$value\""}
                    }
            };

            //adding filters if there is
            GetFilterCollection(true)?.ToList().ForEach(x => req.Params.Add(x.Key, x.Value));

            RESTHelper.Get(req, res =>
            {
                callbackHandler.successCallback?.Invoke(res.Text); //in JSON
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }


        public FirebaseREST LimitToFirst(string value)
        {
            limitToFirstValue = value;
            return new FirebaseREST(path, ToFirebaseFilters());
        }

        public FirebaseREST LimitToLast(string value)
        {
            limitToLastValue = value;
            return new FirebaseREST(path, ToFirebaseFilters());
        }

        public FirebaseREST StartAt(string value)
        {
            startAtValue = value;
            return new FirebaseREST(path, ToFirebaseFilters());
        }

        public FirebaseREST EndAt(string value)
        {
            endAtValue = value;
            return new FirebaseREST(path, ToFirebaseFilters());
        }

        public FirebaseREST EqualTo(string value)
        {
            equalToValue = value;
            return new FirebaseREST(path, ToFirebaseFilters());
        }

        Dictionary<string, string> GetFilterCollection(bool appendQuote)
        {
            Dictionary<string, string> filters = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(limitToFirstValue))
                filters.Add("limitToFirst", limitToFirstValue);

            if (!String.IsNullOrEmpty(limitToLastValue))
                filters.Add("limitToLast", limitToLastValue);

            if (!String.IsNullOrEmpty(startAtValue))
                filters.Add("startAt", appendQuote ? $"\"{startAtValue}\"" : startAtValue);

            if (!String.IsNullOrEmpty(endAtValue))
                filters.Add("endAt", appendQuote ? $"\"{endAtValue}\"" : endAtValue);

            if (!String.IsNullOrEmpty(equalToValue))
                filters.Add("equalTo", equalToValue);

            return filters;
        }

        FirebaseFilters ToFirebaseFilters()
        {
            return new FirebaseFilters
            {
                limitToFirstValue = this.limitToFirstValue,
                limitToLastValue = this.limitToLastValue,
                startAtValue = this.startAtValue,
                endAtValue = this.endAtValue,
                equalToValue = this.equalToValue
            };
        }

        void FromFirebaseFilters(FirebaseFilters filters)
        {
            limitToFirstValue = filters.limitToFirstValue;
            limitToLastValue = filters.limitToLastValue;
            startAtValue = filters.startAtValue;
            endAtValue = filters.endAtValue;
            equalToValue = filters.equalToValue;
        }
    }

    internal class FirebaseFilters 
    {
        internal string limitToFirstValue;
        internal string limitToLastValue;
        internal string startAtValue;
        internal string endAtValue;
        internal string equalToValue;

        internal FirebaseFilters() { }                

        internal FirebaseFilters(string limitToFirstValue, string limitToLastValue, string startAtValue, string endAtValue, string equalToValue) 
        {
            this.limitToFirstValue = limitToFirstValue;
            this.limitToLastValue = limitToLastValue;
            this.startAtValue = startAtValue;
            this.endAtValue = endAtValue;
            this.equalToValue = equalToValue;
        }
    }
}
