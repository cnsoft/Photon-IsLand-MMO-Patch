using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections;
//#if UNITY_EDITOR
using UnityEditor;
using Debug = UnityEngine.Debug;

[CustomEditor(typeof(GuidProperty ))]
[CanEditMultipleObjects]
public class GuidEditor : Editor 
{
	// Use this for initialization
	void Start () {
	     
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	GameObject obj;
	GuidProperty objScript;
 
    void OnEnable() 
    {
       obj = Selection.activeGameObject;
       objScript = obj.GetComponent<GuidProperty>();
	}	
	
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
		if (GUILayout.Button("Query GUID"))
        {
            objScript.ReadUUID();
            Repaint();
        }       
        if (GUILayout.Button("Refresh GUID"))
        {
		 	foreach(GameObject obj in Selection.gameObjects)//Support multiply selection
         	{
				objScript = obj.GetComponent<GuidProperty>();
            	objScript.doUpdate(true); //forced new guid
			}	
            Repaint();
        }       
        GUILayout.EndHorizontal();

        //SerializedProperty sp = serializedObject.FindProperty("RpcList");
        //EditorGUILayout.PropertyField(sp, true);

        if (GUI.changed)
        {
           EditorUtility.SetDirty(target);
        }
		serializedObject.ApplyModifiedProperties();
    }

}
//#endif
