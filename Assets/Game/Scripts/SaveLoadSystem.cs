using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Scripts
{
    public static class SaveLoadSystem
    {
        private static string SavePath => Application.persistentDataPath + "/match_save.json";

        public static void Save(MatchSaveData data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(SavePath, json);
        }

        public static MatchSaveData Load()
        {
            if (!File.Exists(SavePath))
                return null;

            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<MatchSaveData>(json);
        }

        public static void ClearSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }

    [System.Serializable]
    public class MatchSaveData
    {
        public GridData GridData;

        public float TimeRemaining;
        public int Score;
    }

    [System.Serializable]
    public class GridData
    {
        public int GridX;
        public int GridY;

        public List<int> CardIds;        
        public List<bool> MatchedState; 
    }
}