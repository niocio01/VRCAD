using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JsonTest))]
public class JsonTestEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        JsonTest JsonTestScript = (JsonTest)target;
        if (GUILayout.Button("Serialize and Print Test Json"))
        {
            JsonTestScript.PrintTestJson();
        }

        if (GUILayout.Button("Read Json string"))
        {
            JsonTestScript.ReadJson();
        }
    }
}
