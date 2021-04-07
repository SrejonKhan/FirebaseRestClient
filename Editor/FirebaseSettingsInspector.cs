using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FirebaseRestClient.Editor 
{
    [CustomEditor(typeof(FirebaseSettings))]
    public class FirebaseSettingsInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save as JSON", GUILayout.Height(30)))
            {
                string path = EditorUtility.SaveFilePanel("Save Firebase Configuration", "", "FirebaseConfig.json", "json");
                if (!string.IsNullOrEmpty(path)) FirebaseSettings.SaveToJson(path);
            }
            if (GUILayout.Button("Load from JSON", GUILayout.Height(30))) 
            {
                string path = EditorUtility.OpenFilePanel("Open Firebase Configuration", "", "json");
                if (!string.IsNullOrEmpty(path)) FirebaseSettings.LoadFromJson(path);
            }

            GUILayout.EndHorizontal();
        }
    }
}

