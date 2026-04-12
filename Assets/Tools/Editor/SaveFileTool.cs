using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveFileTool : EditorWindow
{
    [MenuItem("Tools/Custom Tools/Save File Tool")]

    public static void ShowWindow(){
        GetWindow(typeof(SaveFileTool));
    }

    private SaveData data;

    private void OnGUI()
    {
        GUILayout.Label("Save File Tool");

        if (data != null)
        {
            data.totScore = EditorGUILayout.IntField("Total score", data.totScore);
            data.highScore = EditorGUILayout.IntField("High score", data.highScore);

            data.currentFloor = EditorGUILayout.IntField("current floor", data.currentFloor);
            data.currentBuilding = EditorGUILayout.IntField("current building", data.currentBuilding);
            
            GUILayout.Label("Is on roof: " + data.isOnRoof);
            GUILayout.Label("Total runs: " + data.runCount);

            data.sprintModeIndex = EditorGUILayout.IntField("current sprint mode index", data.sprintModeIndex);
        }

        if (GUILayout.Button("Load")) { LoadData(Utils.GetSaveFilePath()); }
        if (GUILayout.Button("Save")) { SaveJson(Utils.GetSaveFilePath()); }

        if (GUILayout.Button("clear save")) { ClearFile(); }


    }

    private void LoadData(string path)
    {
        if (data == null) { data = new SaveData(); }

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, data);
        }
        else
        {
            Debug.LogError("no save file found at path + " + path);
        }
    }

    private void SaveJson(string path)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    private void ClearFile()
    {
        SaveData newSave = new SaveData();
        newSave.LoadGeneric();
        data = newSave;

        SaveJson(Utils.GetSaveFilePath());
    }
}
