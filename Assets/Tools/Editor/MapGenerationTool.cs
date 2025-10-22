using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapGenerationTool : EditorWindow
{
    private GameObject mapGenerator;
    private int floor;
    private int building;

    [MenuItem("Tools/Custom Tools/Map Generator Tool")]

    public static void ShowWindow(){
        GetWindow(typeof(MapGenerationTool));
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Generation Tool");

        floor = EditorGUILayout.IntField("floor", floor);
        building = EditorGUILayout.IntField("building", building);

        if (GUILayout.Button("Generate")) { Selection.activeGameObject?.GetComponent<GenerationHandler>()?.Generate(floor, building); }
        if (GUILayout.Button("Clear")) { Selection.activeGameObject?.GetComponent<GenerationHandler>()?.Clear(); }
    }
}
