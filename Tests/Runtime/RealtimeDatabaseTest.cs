using FirebaseRestClient;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace FirebaseRestClient.Tests.Runtime
{
    public class RealtimeDatabaseTest
    {
        //TODO - Write all RDB methods test
        [UnityTest, Order(0)]
        public IEnumerator WriteValuePrimitive()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            firebase.Child("unitTest/writeValuePrimitive").WriteValue("123").
                OnSuccess(() => isDone = true).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x +"\n");
                    //Debug.Log(errorMsg.);
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }
        }

        [UnityTest, Order(1)]
        public IEnumerator WriteValueObject()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            var obj = new TestObject();

            firebase.Child("unitTest/writeValueObject").WriteValue(obj).
                OnSuccess(() => isDone = true).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    //Debug.Log(errorMsg.);
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }
        }

        [UnityTest, Order(2)]
        public IEnumerator WriteValueJson()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            string json = JsonUtility.ToJson(new TestObject());

            firebase.Child("unitTest/writeValueJson").WriteValue(json).
                OnSuccess(() => isDone = true).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }
        }

        [UnityTest, Order(3)]
        public IEnumerator WriteKeyValuePair()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            firebase.Child("unitTest/writeKeyValuePair").WriteKeyValuePair("abc", 123).
                OnSuccess(() => isDone = true).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }
        }

        [UnityTest, Order(4)]
        public IEnumerator WriteValueAsDictionary()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            var dictionary = new Dictionary<string, int>() 
            {
                {"orange", 145},
                {"banana", 456 },
                {"apple", 598}
            };

            firebase.Child("unitTest/writeValueAsDictionary").WriteValueAsDictionary(dictionary).
                OnSuccess(() => isDone = true).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }
        }

        [UnityTest, Order(5)]
        public IEnumerator WriteValueAsDictionaryObject()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            var dictionary = new Dictionary<string, TestObject>()
            {
                {"1", new TestObject(1, "orange")},
                {"2",new TestObject(2, "apple")},
                {"3", new TestObject(3, "mango")}
            };

            firebase.Child("unitTest/writeValueAsDictionaryObject").WriteValueAsDictionary(dictionary).
                OnSuccess(() => isDone = true).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }
        }

        [UnityTest, Order(6)]
        public IEnumerator RawRead()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            string jsonExpected = JsonUtility.ToJson(new TestObject());
            string jsonResult = "";

            firebase.Child("unitTest/writeValueObject").RawRead().
                OnSuccess(res => 
                {
                    isDone = true;
                    jsonResult = res;
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    //Debug.Log(errorMsg.);
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
                Assert.Fail(errorMsg);

            Assert.AreEqual(jsonExpected, jsonResult);
        }

        [UnityTest, Order(7)]
        public IEnumerator Read()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            TestObject result = null;

            firebase.Child("unitTest/writeValueObject").Read<TestObject>().
                OnSuccess(res =>
                {
                    isDone = true;
                    result = res;
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
                Assert.Fail(errorMsg);

            Assert.IsNotNull(result, "Could not read properly or encountered issue while parsing response to desired type.");
        }

        [UnityTest, Order(8)]
        public IEnumerator ReadValue()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            string result = null;

            firebase.Child("unitTest/writeValuePrimitive").ReadValue().
                OnSuccess(res =>
                {
                    isDone = true;
                    result = (string)res;
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
                Assert.Fail(errorMsg);

            Assert.IsNotNull(result, "Could not read properly or encountered issue while parsing response to desired type.");
        }

        [UnityTest, Order(9)]
        public IEnumerator ReadKeyValuePairs()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            Dictionary<string, int> result = null;

            firebase.Child("unitTest/writeValueAsDictionary").ReadKeyValuePairs().
                OnSuccess(res =>
                {
                    isDone = true;
                    result = res.ToDictionary(x => x.Key, x => Convert.ToInt32(x.Value));
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
                Assert.Fail(errorMsg);

            Assert.IsNotNull(result, "Could not read key-value pairs properly.");
            //type-check
            result.ToList().ForEach(x => Assert.AreEqual(typeof(int), x.Value.GetType(), "Could not parse response to desired type or unknow issue occured."));
        }

        [UnityTest, Order(10)]
        public IEnumerator ReadKeyValuePairsObject()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            Dictionary<string, TestObject> result = null;

            firebase.Child("unitTest/writeValueAsDictionaryObject").ReadKeyValuePairs<TestObject>().
                OnSuccess(res =>
                {
                    isDone = true;
                    result = res.ToDictionary(x => x.Key, x => x.Value);
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
                Assert.Fail(errorMsg);

            Assert.IsNotNull(result, "Could not read key-value pairs properly.");
            //type-check
            result.ToList().ForEach(x => Assert.AreEqual(typeof(TestObject), x.Value.GetType(), "Could not parse response to desired type or unknow issue occured."));
            //object-check
            result.ToList().ForEach(x => Assert.IsNull(x.Value, "Could not parse response to desired type or unknow issue occured."));
        }

        [UnityTest, Order(11)]
        public IEnumerator PushObject()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";
            string pushId = null;
            var obj = new TestObject();

            firebase.Child("unitTest/push").Push(obj).
                OnSuccess(res => 
                { 
                    isDone = true;
                    pushId = res;                    
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    //Debug.Log(errorMsg.);
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
                Assert.Fail(errorMsg);

            Assert.IsNotNull(pushId, "Encountered issue while Pushing. PushID is null.");
        }

        [UnityTest, Order(12)]
        public IEnumerator PushJson()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";
            string pushId = null;
            
            string json = JsonUtility.ToJson(new TestObject(1,"PushedName"));

            firebase.Child("unitTest/push").Push(json).
                OnSuccess(res =>
                {
                    isDone = true;
                    pushId = res;
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    //Debug.Log(errorMsg.);
                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }

            Assert.IsNotNull(pushId, "Encountered issue while Pushing. PushID is null.");
        }

        [UnityTest, Order(13)]
        public IEnumerator UpdateObject()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            var obj = new TestObject(1, "Updater");

            firebase.Child("unitTest/updateObject").WriteValue(obj).OnSuccess(() =>
            {
                //change obj values
                obj.id = 100;
                obj.name = "UpdatedNow";

                //Update new value
                firebase.Child("unitTest/updateObject").Update(obj).OnSuccess(() =>
                {
                    //Read and Check
                    firebase.Child("unitTest/updateObject").Read<TestObject>().OnSuccess(res =>
                    {
                        isDone = true;
                        Assert.AreEqual(obj, res, "Couldn't update properly.");
                    }).
                    OnError(err =>
                    {
                        RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                        isDone = true;
                    });
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            }).
            OnError(err => 
            {
                RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }
        }

        [UnityTest, Order(14)]
        public IEnumerator UpdatePrimitiveObject()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            int obj = 123;

            firebase.Child("unitTest/updateObjectPrimitive").WriteValue(obj).OnSuccess(() =>
            {
                //change obj values
                obj = 321;

                //Update new value
                firebase.Child("unitTest/updateObjectPrimitive").Update(obj).OnSuccess(() =>
                {
                    //Read and Check
                    firebase.Child("unitTest/updateObjectPrimitive").ReadValue().OnSuccess(res =>
                    {
                        isDone = true;
                        Assert.AreEqual(obj, res, "Couldn't update properly.");
                    }).
                    OnError(err =>
                    {
                        RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                        isDone = true;
                    });
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            }).
            OnError(err =>
            {
                RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
            {
                Assert.Fail(errorMsg);
            }
        }

        [UnityTest, Order(15)]
        public IEnumerator UpdateObjectJson()
        {
            var firebase = new RealtimeDatabase();
            bool isDone = false;
            string errorMsg = "";

            string json = JsonUtility.ToJson(new TestObject(1, "Updater"));
            string result = "";
            firebase.Child("unitTest/updateObjectJson").WriteValue(json).OnSuccess(() =>
            {
                //change obj values
                json = JsonUtility.ToJson(new TestObject(123, "NewUpdated"));

                //Update new value
                firebase.Child("unitTest/updateObjectJson").Update(json).OnSuccess(() =>
                {
                    //Read and Check
                    firebase.Child("unitTest/updateObjectJson").RawRead().OnSuccess(res =>
                    {
                        result = res;
                        isDone = true;
                    }).
                    OnError(err =>
                    {
                        RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                        isDone = true;
                    });
                }).
                OnError(err =>
                {
                    RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                    isDone = true;
                });

            }).
            OnError(err =>
            {
                RequestErrorHelper.ToDictionary(err).ToList().ForEach(x => errorMsg += x + "\n");
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            if (errorMsg.Length > 0)
                Assert.Fail(errorMsg);

            Assert.AreEqual(json, result, "Couldn't update properly.");
        }
    }

    internal class TestObject 
    {
        public int id = 123;
        public string name = "default";

        public TestObject() { }
        public TestObject(int id, string name) 
        {
            this.id = id;
            this.name = name;
        }
    }
}
