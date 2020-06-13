//############################\\
//Copyrights (c) DarkSky Inc. \\
//Copyrights (c) Only Me Game \\
// https://onlymegame.com     \\
//############################\\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class EasyFpsManager : EditorWindow
{
    [MenuItem("Tools/Easy Fps Counter/Manager")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<EasyFpsManager>("Easy Fps Manager");
    }

    Object cus = EasyFpsCounter.cusPref;

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Welcome in the Easy FPS Counter Manager", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUI.Box(new Rect(0, 30, 10000, 70),"");
        
        if (GUILayout.Button("Create Basic Counter"))
        {
            // Debug.Log("Basic Counter Created");
            if (GameObject.Find("Easy FPS Counter") != null)
            {
                GameObject obj1 = Instantiate(Resources.Load("BasicCounter")) as GameObject;
                obj1.name = "BasicCounter";
                obj1.transform.parent = GameObject.Find("Easy FPS Counter").transform;
            }
            else
            {
                GameObject obj1 = Instantiate(Resources.Load("Easy FPS Counter")) as GameObject;
                obj1.name = "Easy FPS Counter";
                GameObject obj2 = Instantiate(Resources.Load("BasicCounter")) as GameObject;
                obj2.name = "BasicCounter";
                obj2.transform.parent = GameObject.Find("Easy FPS Counter").transform;
            }
        }
        GUILayout.Label("A basic grey counter for prototyping (Use 'Create Prototyping \nCounter' is faster)");
        GUI.Box(new Rect(0, 110, 10000, 70), "");
        GUILayout.Space(25);
        if (GUILayout.Button("Create Sci Fi Counter"))
        {
            //   Debug.Log("Basic Counter Created");
            if (GameObject.Find("Easy FPS Counter") != null)
            {
                GameObject obj1 = Instantiate(Resources.Load("SciFiCounter")) as GameObject;
                obj1.name = "SciFiCounter";
                obj1.transform.parent = GameObject.Find("Easy FPS Counter").transform;
            }
            else
            {
                GameObject obj1 = Instantiate(Resources.Load("Easy FPS Counter")) as GameObject;
                obj1.name = "Easy FPS Counter";
                GameObject obj2 = Instantiate(Resources.Load("SciFiCounter")) as GameObject;
                obj2.name = "SciFiCounter";
                obj2.transform.parent = GameObject.Find("Easy FPS Counter").transform;
            }
        }
        GUILayout.Label("A great blue counter for Sci Fi games");
        GUI.Box(new Rect(0, 110+70*1+10, 10000, 70), "");
        GUILayout.Space(40);
        if (GUILayout.Button("Create Nvidia Counter"))
        {
            // Debug.Log("Basic Counter Created");
            if (GameObject.Find("Easy FPS Counter") != null)
            {
                GameObject obj1 = Instantiate(Resources.Load("NvidiaCounter")) as GameObject;
                obj1.name = "NvidiaCounter";
                obj1.transform.parent = GameObject.Find("Easy FPS Counter").transform;
            }
            else
            {
                GameObject obj1 = Instantiate(Resources.Load("Easy FPS Counter")) as GameObject;
                obj1.name = "Easy FPS Counter";
                GameObject obj2 = Instantiate(Resources.Load("NvidiaCounter")) as GameObject;
                obj2.name = "NvidiaCounter";
                obj2.transform.parent = GameObject.Find("Easy FPS Counter").transform;
            }
        }
        GUILayout.Label("A cubic counter with Nvidia's colors");
        GUI.Box(new Rect(0, 110 + 70 * 2 + 10*2, 10000, 70), "");
        GUILayout.Space(40);
        if (GUILayout.Button("Create Western Counter"))
        {
            // Debug.Log("Basic Counter Created");
            if (GameObject.Find("Easy FPS Counter") != null)
            {
                GameObject obj1 = Instantiate(Resources.Load("WesternCounter")) as GameObject;
                obj1.name = "WesternCounter";
                obj1.transform.parent = GameObject.Find("Easy FPS Counter").transform;
            }
            else
            {
                GameObject obj1 = Instantiate(Resources.Load("Easy FPS Counter")) as GameObject;
                obj1.name = "Easy FPS Counter";
                GameObject obj2 = Instantiate(Resources.Load("WesternCounter")) as GameObject;
                obj2.name = "WesternCounter";
                obj2.transform.parent = GameObject.Find("Easy FPS Counter").transform;
            }
        }
        GUILayout.Label("A counter for all western games!");
        GUI.Box(new Rect(0, 110 + 70 * 3 + 10 * 3, 10000, 70), "");
        GUILayout.Space(40);
        if (GUILayout.Button("Create Custom Counter"))
        {
            // Debug.Log("Basic Counter Created");
            if(cus != null)
            {
                if (GameObject.Find("Easy FPS Counter") != null)
                {
                    GameObject obj1 = Instantiate(cus) as GameObject;
                    obj1.name = cus.name;
                    obj1.transform.parent = GameObject.Find("Easy FPS Counter").transform;
                }
                else
                {
                    GameObject obj1 = Instantiate(Resources.Load("Easy FPS Counter")) as GameObject;
                    obj1.name = "Easy FPS Counter";
                    GameObject obj2 = Instantiate(Instantiate(cus)) as GameObject;
                    obj2.name = cus.name;
                    obj2.transform.parent = GameObject.Find("Easy FPS Counter").transform;
                }
            }
        }
        GUILayout.Label("Spawn your own counter (beta)");
        cus = EditorGUILayout.ObjectField("Counter Prefab:", cus, typeof(GameObject),false);
        EasyFpsCounter.cusPref = cus;
       
    }

    [MenuItem("Tools/Easy Fps Counter/Create Prototyping Counter")]
    private static void SpawnProt()
    {
        if (GameObject.Find("Easy FPS Counter") != null)
        {
            GameObject obj1 = Instantiate(Resources.Load("BasicCounter")) as GameObject;
            obj1.name = "BasicCounter";
            obj1.transform.parent = GameObject.Find("Easy FPS Counter").transform;
        }
        else
        {
            GameObject obj1 = Instantiate(Resources.Load("Easy FPS Counter")) as GameObject;
            obj1.name = "Easy FPS Counter";
            GameObject obj2 = Instantiate(Resources.Load("BasicCounter")) as GameObject;
            obj2.name = "BasicCounter";
            obj2.transform.parent = GameObject.Find("Easy FPS Counter").transform;
        }
    }
    [MenuItem("Tools/Easy Fps Counter/Open Doc")]
    private static void OpenDoc()
    {
        Process.Start("https://assets.onlymegame.com/EFPSC/Easy_FPS_Counter_Documentation.pdf");
    }

}
