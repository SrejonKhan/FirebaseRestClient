using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace FirebaseRestClient.Editor
{
    internal class FirebaseSettingsEditor 
    {
        [SettingsProvider]
        internal static SettingsProvider CustomSettinsProvider()
        {
            var provider = new SettingsProvider("Project/Firebase Rest Client", SettingsScope.Project)
            {
                guiHandler = context => UnityEditor.Editor.CreateEditor(FirebaseSettings.Instance).OnInspectorGUI(),
                keywords = SettingsProvider.GetSearchKeywordsFromSerializedObject(new SerializedObject(FirebaseSettings.Instance))
            };

            return provider;
        }
    }
}
#endif