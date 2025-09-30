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

    private void OnGUI()
    {
        GUILayout.Label("Save File Tool");

        // Scores - get scores, set high and total score
        // Game - get game status, and set floor + building - Try to update IsOnRoof for consistency.

        if (GUILayout.Button("Print Scores")) { PrintScores(); }
        if (GUILayout.Button("Print Stats")) { PrintGameStats(); }

        //itemName = EditorGUILayout.TextField(itemName);
        //itemCount = EditorGUILayout.IntField(itemCount);
        //if (GUILayout.Button("Add Item to Inventory")) { AddItemInventory(); }
        //if (GUILayout.Button("Print Inventory")) { PrintInventory(); }

    }

    private SaveData LoadData(string path)
    {
        SaveData data = new SaveData(); // load save file
        string json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, data);

        return data;
    }

    private void SaveJson(SaveData data, string path)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    private void PrintScores()
    {
        SaveData data = LoadData(Utils.GetSaveFilePath());
        if (data != null)
        {
            Debug.Log("Game's Total Score: " + data.totScore);
            Debug.Log("Game's High Score: " + data.highScore);
            Debug.Log("Game's Total Runs: " + data.runCount);
        }
    }

    private void PrintGameStats()
    {
        SaveData data = LoadData(Utils.GetSaveFilePath());
        if (data != null)
        {
            Debug.Log("Game current floor: " + data.currentFloor);
            Debug.Log("Game current building: " + data.currentBuilding);
            Debug.Log("Game is on roof?: " + data.isOnRoof);
        }
    }
}
