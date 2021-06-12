using System.Collections;
using System.Collections.Generic;
using FirebaseRestClient;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


namespace FirebaseRestClient.Tests.Editor
{
    public class ConfigurationTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void FileExistance()
        {
            var config = Resources.Load<FirebaseSettings>("FirebaseRestClient");
            Assert.IsTrue(config, "Config File Could not found. Please Open Configuration in Project Settings -> Firebase Rest Client");
        }
    }
}


