//############################\\
//Copyrights (c) DarkSky Inc. \\
//Copyrights (c) Only Me Game \\
// https://onlymegame.com     \\
//############################\\


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

[CustomEditor(typeof(EasyFps))]
[CanEditMultipleObjects]
public class EasyFpsEditor : Editor {

    int frtoset;
    SerializedProperty Less10;
    SerializedProperty Less30;
    SerializedProperty Less60;
    SerializedProperty Less120;
    SerializedProperty MoreMax;
    bool ShowEvents;
    

    void OnEnable()
    {
        Less10 = serializedObject.FindProperty("OnFpsLessThan10");
        Less30 = serializedObject.FindProperty("OnFpsLessThan30");
        Less60 = serializedObject.FindProperty("OnFpsLessThan60");
        Less120 = serializedObject.FindProperty("OnFpsLessThan120");
        MoreMax = serializedObject.FindProperty("OnFpsMoreThanMax");
    }

    public override void OnInspectorGUI()
    {
        EasyFps script = (EasyFps)target;
        script.ncm = EditorGUILayout.Toggle("NoCodingMode", script.ncm);
               if (script.ncm)
        {
            GUILayout.Label("You can always acces Easy FPS Counter by coding. (see doc)");
            script.maxFR = EditorGUILayout.IntField("Max Framerate:", script.maxFR);
            script.refresht = EditorGUILayout.FloatField("Refresh Time:", script.refresht);
        }
        else
        {
           
            GUILayout.Label("see the doc for further details");
            if(GUILayout.Button("Open Doc"))
            {
                Process.Start("https://assets.onlymegame.com/EFPSC/Easy_FPS_Counter_Documentation.pdf");
            }
        }
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Separator();
        
         frtoset = EditorGUILayout.IntField("FrameRate", frtoset);
        if (GUILayout.Button("Set Max FrameRate"))
        {
            EasyFpsCounter.EasyFps.MaxFrameRate = frtoset;
        }
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Separator();
        //GUILayout.Label("Custom Events");
        //ShowEvents = GUILayout.Toggle(ShowEvents, "Show Events");
        ShowEvents = EditorGUILayout.Foldout(ShowEvents, "Custom Events");
        if (ShowEvents)
        {
            EditorGUILayout.PropertyField(Less10, new GUIContent("When FPS is less than 10 (1 dispatch)"));
            EditorGUILayout.PropertyField(Less30, new GUIContent("When FPS is less than 30 (1 dispatch)"));
            EditorGUILayout.PropertyField(Less60, new GUIContent("When FPS is less than 60 (1 dispatch)"));
            EditorGUILayout.PropertyField(Less120, new GUIContent("When FPS is less than 120 (1 dispatch)"));
            EditorGUILayout.PropertyField(MoreMax, new GUIContent("When FPS is more than Max FrameRate (1 dispatch) (61 instead of 60)"));
        }
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Separator();

            EditorGUILayout.LabelField("For Text Mesh Pro Integration, please watch the documentation");
        if (GUILayout.Button("Open Doc"))
        {
            Process.Start("https://assets.onlymegame.com/EFPSC/Easy_FPS_Counter_Documentation.pdf");
        }
        EditorGUILayout.Separator();
        serializedObject.ApplyModifiedProperties();
    }
}
