using System;
using System.IO;
using UnityEngine;

// this handles saving and loading the data from the binary file itself
// formatting the data is done seperately by the SaveData class

public static class SaveManager
{
    private const string fileName = "/player.json";

    public static void Save(GameStatus status)
    {
        string path = Application.persistentDataPath + fileName; // get the file path

        SaveData data = new SaveData();
        data.loadFromGameStatus(status); // create save data class and load players data onto it
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json); // save to file
    }

    public static SaveData Load()
    {
        string path = Application.persistentDataPath + fileName; // get the file path
        if (File.Exists(path))
        {
            SaveData data = new SaveData();
            string json = File.ReadAllText(path);
            //Debug.Log(json);
            JsonUtility.FromJsonOverwrite(json, data);
            return data;
        }
        else
        {
            SaveData data = new SaveData();
            data.loadBasics();
            Debug.LogError("no save file found at path - creating new save file at == " + path);

            // create file path to ensure this doesnt happen again
            Directory.CreateDirectory(Application.persistentDataPath); 
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);

            return data;
        }
    }

}
