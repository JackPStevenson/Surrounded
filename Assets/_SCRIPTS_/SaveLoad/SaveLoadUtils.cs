using System;
using System.IO;
using UnityEngine;

public static class SaveLoadUtils {
    
    public static bool TrySave<T>(string savePath, T info) where T : class {
        try {
            using StreamWriter writer = new StreamWriter(savePath, false);
            writer.Write(JsonUtility.ToJson(info));
        }
        catch (Exception e) {
            return false;
        }
        
        //Debug.Log("Success! Saved " + typeof(T) + ".");
        return true;
    }

    public static bool TryLoad<T>(string loadPath, out T result) where T : class {
        result = null;

        try {
            using StreamReader reader = new StreamReader(loadPath);
            result = JsonUtility.FromJson<T>(reader.ReadToEnd());
        }
        catch (Exception e) {
            return false;
        }
        
        //Debug.Log("Success! Loaded " + typeof(T) + ".");
        return true;
    }
}