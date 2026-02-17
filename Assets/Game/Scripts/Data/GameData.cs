using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
    public class GameData : ScriptableObject
    {
        public int MatchDuration;
        public int ScoreMultiplier;
        public List<CardData> Cards = new List<CardData>();
    }

    [Serializable]
    public class CardData
    {
        public int ID;
        public Sprite Sprite;
    }
}