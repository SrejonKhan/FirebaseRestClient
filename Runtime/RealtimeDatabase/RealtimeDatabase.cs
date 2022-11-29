using FirebaseRestClient.Helper;
using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace FirebaseRestClient
{
    public class RealtimeDatabase
    {
        private string path; /// path from root

        //Filters
        private string limitToFirstValue;
        private string limitToLastValue;
        private string startAtValue;
        private string endAtValue;
        private string equalToValue;

        /// <summary>
        /// Create Firebase Realtime Database Instance
        /// </summary>
        public RealtimeDatabase() 
        {
            FirebaseSettings.LoadSettings();
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="path">Previous Path</param>
        internal RealtimeDatabase(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="path">Previous Path</param>
        /// <param name="filters">Filters</param>
        internal RealtimeDatabase(string path, FirebaseFilters filters)
        {
            this.path = path;
            FromFirebaseFilters(filters);
        }

        /// <summary>
        /// Point to Child Node
        /// </summary>
        /// <param name="value">Path</param>
        /// <returns></returns>
        public RealtimeDatabase Child(string value)
        {
            string newPath = String.IsNullOrEmpty(path) ? value : $"{path}/{value}";
            return new RealtimeDatabase(newPath, ToFirebaseFilters());
        }

        /// <summary>
        /// Write Key-Value Pair to specified node
        /// </summary>
        /// <param name="key">Key Name</param>
        /// <param name="value">Value, any datatypes</param>
        /// <param name="append">Should append to existing list or just overwrite?</param>
        /// <returns></returns>
        public GeneralCallback WriteKeyValuePair(string key, object value, bool append = false)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string json = GenerateJson(value, key);

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + path + ".json" + GetAuthParam(),
                BodyString = json
            };

            if (append) //patch
            {
                RESTHelper.Patch(req, res => callbackHandler.successCallback?.Invoke(), 
                    err => callbackHandler.exceptionCallback?.Invoke(err));
                return callbackHandler;
            }

            RESTHelper.PutJson(req, res => callbackHandler.successCallback?.Invoke(), 
                err => callbackHandler.exceptionCallback?.Invoke(err));

            return callbackHandler;
        }

        /// <summary>
        /// Write to specified node
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public GeneralCallback WriteValue(object value)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string json = GenerateBody(value).ToString();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + path + ".json" + GetAuthParam(),
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

        /// <summary>
        /// Write Dictionary to specified reference
        /// </summary>
        /// <typeparam name="T1">Value type</typeparam>
        /// <param name="dictionary">Dictionary that will be written</param>
        /// <returns></returns>
        public GeneralCallback WriteValueAsDictionary<T1>(Dictionary<string, T1>dictionary)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string json = SerializerHelper.Serialize(typeof(Dictionary<string,T1>), dictionary);
 
            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + path + ".json" + GetAuthParam(),
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

        /// <summary>
        /// Generate Firebase Push ID
        /// </summary>
        /// <returns>Unique Push ID</returns>
        public string GeneratePushID()
        {
            return PushIDGenerator.GeneratePushID();
        }

        /// <summary>
        /// Convert Push ID
        /// </summary>
        /// <param name="pushId">Push ID</param>
        /// <returns></returns>
        public long ConvertPushID(string pushId)
        {
            return PushIDGenerator.ConvertPushID(pushId);   
        }

        /// <summary>
        /// Push Raw Json to specified node
        /// </summary>
        /// <param name="json">Valid Raw Json</param>
        /// <returns>Generated Push ID</returns>
        public StringCallback Push(string json)
        {
            StringCallback callbackHandler = new StringCallback();

            string uid = GeneratePushID();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + path + "/" + uid + ".json" + GetAuthParam(),
                BodyString = json
            };

            RESTHelper.PutJson(req, res =>
            {
                callbackHandler.successCallback?.Invoke(uid); //UID
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Push object as Body
        /// </summary>
        /// <typeparam name="T">Type of Body</typeparam>
        /// <param name="body">Body</param>
        /// <returns>Generated Push ID</returns>
        public StringCallback Push<T>(T body)
        {
            StringCallback callbackHandler = new StringCallback();

            string uid = GeneratePushID();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + path + "/" + uid + ".json" + GetAuthParam(),
                Body = body
            };

            RESTHelper.Put<T>(req, res =>
            {
                callbackHandler.successCallback?.Invoke(uid); //UID
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Remove value of specified Node
        /// </summary>
        /// <returns></returns>
        public GeneralCallback Remove()
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string route = FirebaseConfig.endpoint + path + ".json" + GetAuthParam();

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

        /// <summary>
        /// Read Json Object
        /// </summary>
        /// <typeparam name="T">Desired type to be converted</typeparam>
        /// <returns>Result in object format</returns>
        public ObjectCallback<T> Read<T>()
        {
            ObjectCallback<T> callbackHandler = new ObjectCallback<T>();

            string route = FirebaseConfig.endpoint + path + ".json" + GetAuthParam();

            RESTHelper.Get(route, res =>
            {
                callbackHandler.successCallback?.Invoke(JsonUtility.FromJson<T>(res.Text));
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Read raw Json
        /// </summary>
        /// <param name="shallow">If shallow, all json object will return as true. Better for surface reading.</param>
        /// <returns>Raw Json</returns>
        public StringCallback RawRead(bool shallow=false)
        {
            StringCallback callbackHandler = new StringCallback();

            string route = FirebaseConfig.endpoint + path + ".json" + GetAuthParam();

            if (shallow) route += GetShallowParam(); 

            RESTHelper.Get(route, res =>
            {
                callbackHandler.successCallback?.Invoke(res.Text); 
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Read value of key
        /// </summary>
        /// <returns>Value as Json String, else object</returns>
        public ObjectCallback ReadValue()
        {
            ObjectCallback callbackHandler = new ObjectCallback();

            string route = FirebaseConfig.endpoint + path + ".json" + GetAuthParam();

            RESTHelper.Get(route, res =>
            {
                var resData = fsJsonParser.Parse(res.Text);

                if (resData.IsDictionary)
                {
                    callbackHandler.successCallback?.Invoke(res.Text); //invoke with raw Json, as it's not a key-value pair
                    return;
                }
                //No collection
                object value = resData._value;
                callbackHandler.successCallback?.Invoke(value);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Read key-value pairs
        /// </summary>
        /// <returns>Dictionary, key is string and value is object, it can be any primitive types or fsData</returns>
        public DictionaryCallback<string, object> ReadKeyValuePairs()
        {
            DictionaryCallback<string, object> callbackHandler = new DictionaryCallback<string, object>();

            string route = FirebaseConfig.endpoint + path + ".json" + GetAuthParam();

            RESTHelper.Get(route, res =>
            {
                var resData = fsJsonParser.Parse(res.Text); //in JSON 
                Dictionary<string, object> destructuredRes = new Dictionary<string, object>();

                if (resData.IsDictionary)
                {
                    resData.AsDictionary.ToList().ForEach(x => destructuredRes.Add(x.Key, x.Value._value));
                    callbackHandler.successCallback?.Invoke(destructuredRes);
                    return;
                }
                //No collection, single result (key-value pair)
                string[] splittedPaths = path.Split('/');
                destructuredRes.Add(splittedPaths[splittedPaths.Length - 1], resData._value); //last path as key, resData._value as object
                callbackHandler.successCallback?.Invoke(destructuredRes);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Read key-value pairs
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <returns>Dictionary, key as string, value as specified type. Returns nothing if failed to deserialize.</returns>
        public DictionaryCallback<string, T> ReadKeyValuePairs<T>()
        {
            DictionaryCallback<string, T> callbackHandler = new DictionaryCallback<string, T>();

            string route = FirebaseConfig.endpoint + path + ".json" + GetAuthParam();

            RESTHelper.Get(route, res =>
            {
                var resData = fsJsonParser.Parse(res.Text);

                object deserializedRes = null;

                fsSerializer serializer = new fsSerializer();
                serializer.TryDeserialize(resData, typeof(Dictionary<string, T>), ref deserializedRes);

                Dictionary<string, T> destructuredRes = (Dictionary<string, T>)deserializedRes;

                callbackHandler.successCallback?.Invoke(destructuredRes);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Check for child in selected path
        /// </summary>
        /// <param name="child">child name</param>
        /// <returns>Returns true if child exists, else false</returns>
        public BooleanCallback HasChild(string child)
        {
            BooleanCallback callbackHandler = new BooleanCallback();

            string route = FirebaseConfig.endpoint + path + "/" + child + ".json" + GetAuthParam();

            RESTHelper.Get(route, res =>
            {
                //If there is no child, server return "null"
                if (res.Text != "null" && res.Text.Length > 0)
                {
                    callbackHandler.successCallback?.Invoke(true);
                }
                else
                {
                    callbackHandler.successCallback?.Invoke(false);
                }
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }


        /// <summary>
        /// Update value of specified path
        /// </summary>
        /// <param name="value">Updated value</param>
        /// <returns></returns>
        public GeneralCallback Update(object value)
        {
            GeneralCallback callbackHandler = new GeneralCallback();

            string json = GenerateJson(value);
            string upperNodePath = GetImmediateUpperNode();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + upperNodePath + ".json" + GetAuthParam(),
                BodyString = json
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

        /// <summary>
        /// Union of all server events - ChildAdded, ChildRemoved, ChildChanged
        /// </summary>
        /// <param name="callback"></param>
        public void ValueChanged(Action<ChildEventArgs> callback, bool shallow = false)
        {
            ChildEventHandler childEventHandler = new ChildEventHandler();
            childEventHandler.OnChildEventReceived += callback;
            childEventHandler.childEventType = ChildEventHandler.ChildEventType.ValueChanged;
            childEventHandler.shallow = shallow;
            ChildEventListen(childEventHandler);
        }

        /// <summary>
        /// Listen for Child addition to targeted path and in-depth.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="shallow">Listen to surface level child, not in-depth child</param>
        public void ChildAdded(Action<ChildEventArgs> callback, bool shallow=false)
        {
            ChildEventHandler childEventHandler = new ChildEventHandler();
            childEventHandler.OnChildEventReceived += callback;
            childEventHandler.childEventType = ChildEventHandler.ChildEventType.ChildAdded;
            childEventHandler.shallow = shallow;
            ChildEventListen(childEventHandler);
        }

        /// <summary>
        /// Listen for Child Remove at any level from targeted path.
        /// </summary>
        /// <param name="callback"></param>
        public void ChildRemoved(Action<ChildEventArgs> callback, bool shallow = false)
        {
            ChildEventHandler childEventHandler = new ChildEventHandler();
            childEventHandler.OnChildEventReceived += callback;
            childEventHandler.childEventType = ChildEventHandler.ChildEventType.ChildRemoved;
            childEventHandler.shallow = shallow;
            ChildEventListen(childEventHandler);
        }

        /// <summary>
        /// Combination of both ChildAdded and ChildRemoved.
        /// </summary>
        /// <param name="callback"></param>
        public void ChildChanged(Action<ChildEventArgs> callback, bool shallow = false)
        {
            ChildEventHandler childEventHandler = new ChildEventHandler();
            childEventHandler.OnChildEventReceived += callback;
            childEventHandler.childEventType = ChildEventHandler.ChildEventType.ChildChanged;
            childEventHandler.shallow = shallow;
            ChildEventListen(childEventHandler);
        }


        void ChildEventListen(ChildEventHandler eventHandler)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            RequestHelper req = new RequestHelper
            {
                Headers = new Dictionary<string, string>
                {
                    { "Accept", "text/event-stream"},
                },
                Uri = FirebaseConfig.endpoint + path + ".json" + GetAuthParam(),
                DownloadHandler = eventHandler,
                Retries = int.MaxValue,
                RetrySecondsDelay = 1,
                
            };

            //create an unsubscriber for events
            var unsubscriberGO = GameObject.Find("FrcEventUnsubscriber" + path);
            if (unsubscriberGO == null)
            {
                unsubscriberGO = new GameObject("FrcEventUnsubscriber" + path);
                unsubscriberGO.AddComponent<EventUnsubscriber>().requestHelper = req;
                MonoBehaviour.DontDestroyOnLoad(unsubscriberGO);
            }
            //Error handling are being handled internally
            RESTHelper.Get(req, err => RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => Debug.LogError(x.Key + " - " + x.Value)));

#elif UNITY_WEBGL && !UNITY_EDITOR
            string url = FirebaseConfig.endpoint + path + ".json" + GetAuthParam();
            try
            {
                EventSource es = new EventSource();
                es.Init(url);
                es.OnEventMessage += eventHandler.ReceiveWebGlData;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            var esUnsubscriberGO = GameObject.Find("FrcEventSourceUnsubscriber");
            if (esUnsubscriberGO == null)
            {
                esUnsubscriberGO = new GameObject("FrcEventSourceUnsubscriber");
                esUnsubscriberGO.AddComponent<EventSourceUnsubscriber>();
                MonoBehaviour.DontDestroyOnLoad(esUnsubscriberGO);
            }
#endif
        }

        /// <summary>
        /// Order results by the value of a specified child key
        /// </summary>
        /// <param name="key">Child Key</param>
        /// <returns>Raw Json</returns>
        public StringCallback OrderByChild(string key)
        {
            StringCallback callbackHandler = new StringCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + path + ".json" + GetAuthParam(),
                Params = new Dictionary<string, string>
                        {
                            {"orderBy", "\"" + key + "\""}
                        }
            };

            //adding filters if there is
            GetFilterCollection(false)?.ToList().ForEach(x => req.Params.Add(x.Key, x.Value));

            RESTHelper.Get(req, res =>
            {
                callbackHandler.successCallback?.Invoke(res.Text); 
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Order results by child keys. [Firebase Rules must be set]
        /// </summary>
        /// <returns>Raw Json</returns>
        public StringCallback OrderByKey()
        {
            StringCallback callbackHandler = new StringCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + path + ".json" + GetAuthParam(),
                Params = new Dictionary<string, string>
                    {
                        {"orderBy", "\"$key\""}
                    }
            };

            //adding filters if there is
            GetFilterCollection(true)?.ToList().ForEach(x => req.Params.Add(x.Key, x.Value));

            RESTHelper.Get(req, res =>
            {
                callbackHandler.successCallback?.Invoke(res.Text); 
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Order results by child value. [Firebase rules must be set]
        /// </summary>
        /// <returns>Raw Json</returns>
        public StringCallback OrderByValue()
        {
            StringCallback callbackHandler = new StringCallback();

            RequestHelper req = new RequestHelper
            {
                Uri = FirebaseConfig.endpoint + path + ".json" + GetAuthParam(),
                Params = new Dictionary<string, string>
                    {
                        {"orderBy", "\"$value\""}
                    }
            };

            //adding filters if there is
            GetFilterCollection(true)?.ToList().ForEach(x => req.Params.Add(x.Key, x.Value));

            RESTHelper.Get(req, res =>
            {
                callbackHandler.successCallback?.Invoke(res.Text);
            },
            err =>
            {
                callbackHandler.exceptionCallback?.Invoke(err);
            });

            return callbackHandler;
        }

        /// <summary>
        /// Maximum numbers to return from the beginning
        /// </summary>
        /// <param name="value">Maximum return</param>
        /// <returns></returns>
        public RealtimeDatabase LimitToFirst(int? value)
        {
            limitToFirstValue = value.ToString();
            return new RealtimeDatabase(path, ToFirebaseFilters());
        }

        /// <summary>
        /// Maximum numbers to return from the end
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public RealtimeDatabase LimitToLast(int? value)
        {
            limitToLastValue = value.ToString();
            return new RealtimeDatabase(path, ToFirebaseFilters());
        }

        /// <summary>
        /// Returns items greater than or equal to specified value
        /// </summary>
        /// <param name="value">Filter Value</param>
        /// <returns></returns>
        public RealtimeDatabase StartAt(object value)
        {
            if (value != null) startAtValue = value.GetType() == typeof(string) && !String.IsNullOrEmpty(value.ToString()) ? $"\"{value}\"" : value.ToString(); //append quote if it's string
            else startAtValue = "";
            return new RealtimeDatabase(path, ToFirebaseFilters());
        }

        /// <summary>
        /// Returns items less than or equal specified value
        /// </summary>
        /// <param name="value">Filter Value</param>
        /// <returns></returns>
        public RealtimeDatabase EndAt(object value)
        {
            if (value != null) endAtValue = value.GetType() == typeof(string) && !String.IsNullOrEmpty(value.ToString()) ? $"\"{value}\"" : value.ToString(); //append quote if it's string
            else endAtValue = "";
            return new RealtimeDatabase(path, ToFirebaseFilters());
        }

        /// <summary>
        /// Returns items equal to specified value
        /// </summary>
        /// <param name="value">Filter Value</param>
        /// <returns></returns>
        public RealtimeDatabase EqualTo(object value)
        {
            if (value != null) equalToValue = value.GetType() == typeof(string) && !String.IsNullOrEmpty(value.ToString()) ? $"\"{value}\"" : value.ToString(); //append quote if it's string
            else equalToValue = "";
            return new RealtimeDatabase(path, ToFirebaseFilters());
        }

        /// <summary>
        /// Generate filters for query
        /// </summary>
        /// <param name="appendQuote">Surround string with quote</param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert props to FirebaseFilters
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Assign to props from FirebaseFilters
        /// </summary>
        /// <param name="filters"></param>
        void FromFirebaseFilters(FirebaseFilters filters)
        {
            limitToFirstValue = filters.limitToFirstValue;
            limitToLastValue = filters.limitToLastValue;
            startAtValue = filters.startAtValue;
            endAtValue = filters.endAtValue;
            equalToValue = filters.equalToValue;
        }

        /// <summary>
        /// Get Auth Param if any user is authenticated 
        /// </summary>
        /// <returns>returns auth param, can be null if there's no user authenticated</returns>
        string GetAuthParam()
        {
            return FirebaseAuthentication.currentUser != null ? $"?auth={FirebaseAuthentication.currentUser.accessToken}" : "";
        }

        /// <summary>
        /// Get Shallow Param for optimized RawRead
        /// </summary>
        /// <returns>returns shallow param if there is no authenticated user, else nothing</returns>
        string GetShallowParam()
        {
            return FirebaseAuthentication.currentUser != null ? "&shallow=true" : $"?shallow=true";
        }

        /// <summary>
        /// Generate server friendly Json
        /// </summary>
        /// <param name="value">Any datatypes</param>
        /// <param name="key">Key Value</param>
        /// <returns></returns>
        string GenerateJson(object value, string key = "")
        {
            //Get key
            if (String.IsNullOrEmpty(key))
            {
                string[] splittedPaths = path.Split('/');
                key = splittedPaths[splittedPaths.Length - 1];
            }

            string json = "{" +
            $"\"{key}\":{value}" +
            "}";

            var paramType = value.GetType();
            //We should not handle Array implicitly. It will be poor for end-user while deserializing.
            if (paramType.IsArray) 
            {
                throw new ArgumentException("Parameter cannnot be Array. Convert Array to JSON String and pass as string.");
            }
            //Serializing Dictionary from object needs expensive work-around. We use dictionary focused method.
            if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                throw new ArgumentException("Parameter cannnot be Dictionary. Use Dictionary Methods, e.g WriteValueAsDictionary()");
            }
            //If param is not primitive, convert to json
            if (!paramType.IsPrimitive && paramType != typeof(string))
            {
                string paramJson = JsonUtility.ToJson(value);
                json = "{" +
                $"\"{key}\":{paramJson}" +
                "}";
                return json;
            }
            //if normal string, wrap with quote
            if (paramType == typeof(string))
            {
                string paramString = value.ToString();
                int length = paramString.Length;
                //make sure it's not json, if json, leave as it is.
                if (paramString[0] != '{' && paramString[length - 1] != '}')
                {
                    json = "{" +
                    $"\"{key}\":\"{value}\"" +
                    "}";
                    return json;
                }
            }

            //bool type should to lowercase to work in json
            if (paramType == typeof(bool))
            {
                json = "{" +
                $"\"{key}\":{value.ToString().ToLower()}" +
                "}";
            }
            return json;
        }

        /// <summary>
        /// Generate server friendly body
        /// </summary>
        /// <param name="value">Any datatypes</param>
        /// <returns>Object, in JSON String or any primitive Datatype.</returns>
        object GenerateBody(object value)
        {
            object body = value;
            var paramType = value.GetType();

            //We should not handle Array implicitly. It will be poor for end-user while deserializing.
            if (paramType.IsArray || paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(IList<>))
            {
                throw new ArgumentException("Parameter cannnot be Array. Convert Array to JSON String and pass as string.");
            }
            //Serializing Dictionary from object needs expensive work-around. We use dictionary focused method.
            if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                throw new ArgumentException("Parameter cannnot be Dictionary. Use Dictionary Methods, e.g WriteValueAsDictionary()");
            }
            //If param is not primitive, convert to json
            if (!paramType.IsPrimitive && paramType != typeof(string))
            {
                body = JsonUtility.ToJson(value);
                return body;
            }
            //if normal string, wrap with quote
            if (paramType == typeof(string))
            {
                string paramString = value.ToString();
                int length = paramString.Length;
                //make sure it's not json, if json, leave as it is.
                if (paramString[0] != '{' && paramString[length - 1] != '}' && paramString[length - 1] != '}')
                {
                    body = $"\"{value}\"";
                    return body;
                }
            }
            //bool type should to lowercase to work in json
            if (paramType == typeof(bool))
            {
                body = value.ToString().ToLower();
            }
            return body;
        }

        /// <summary>
        /// Returns upper node from passed node arg
        /// </summary>
        /// <returns></returns>
        string GetImmediateUpperNode()
        {
            //Create immediate upper node path
            string[] splittedPaths = path.Split('/');
            Array.Resize(ref splittedPaths, splittedPaths.Length - 1);
            string upperNodePath = ""; //one level up path
            splittedPaths.ToList().
                ForEach(s => upperNodePath = String.IsNullOrEmpty(upperNodePath) ?
                upperNodePath += s : upperNodePath += "/" + s);

            return upperNodePath;
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
