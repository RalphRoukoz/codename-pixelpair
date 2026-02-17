#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class ClearMatchSaveEditor
{
    private static string SavePath =>
        Application.persistentDataPath + "/match_save.json";

    [MenuItem("Tools/Memory Game/Clear Saved Match")]
    public static void ClearSavedMatch()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Match save data cleared successfully.");
        }
        else
        {
            Debug.Log("No saved match data found.");
        }
    }

    [MenuItem("Tools/Memory Game/Clear Saved Match", true)]
    private static bool ValidateClearSavedMatch()
    {
        return true; 
    }
}
#endif