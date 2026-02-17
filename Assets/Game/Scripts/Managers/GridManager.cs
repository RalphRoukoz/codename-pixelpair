using System.Collections.Generic;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Managers
{
    public class GridManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform m_BoardRoot;
        [SerializeField] private CardPoolManager m_CardPool;
        [SerializeField] private GameData m_GameData;
    
        [Header("Grid Settings")]
        [SerializeField] private int m_MaxGridSizeX;
        [SerializeField] private int m_MaxGridSizeY;
        [SerializeField] private float m_Spacing = 0.2f;
    
        private readonly List<Card> m_ActiveCards = new List<Card>();

        private int m_GridX;
        private int m_GridY;

        public void GenerateGrid(int gridX, int gridY, System.Action<Card> onSelected, out int totalPairs)
        {
            ClearGrid();
        
            m_GridX = gridX;
            m_GridY = gridY;

            int total = m_GridX * m_GridY;
            totalPairs = total / 2;
        
            m_CardPool.Prewarm(total);

            List<CardData> generatedSet = GenerateCardSet(total);

            float cardScale = CalculateCardScale(m_GridX, m_GridY);

            Vector2 offset = new Vector2(
                (m_GridX - 1) * (cardScale + m_Spacing) * 0.5f,
                (m_GridY - 1) * (cardScale + m_Spacing) * 0.5f
            );

            for (int i = 0; i < total; i++)
            {
                Card card = m_CardPool.Get();
                m_ActiveCards.Add(card);

                card.transform.SetParent(m_BoardRoot);
                card.transform.localScale = Vector3.one * cardScale;

                int x = i % m_GridX;
                int y = i / m_GridX;

                card.transform.localPosition = new Vector3(
                    x * (cardScale + m_Spacing) - offset.x,
                    0,
                    -(y * (cardScale + m_Spacing) - offset.y)
                );

                card.Initialize(generatedSet[i], onSelected);
            }
        }

        private List<CardData> GenerateCardSet(int total)
        {
            int pairCount = total / 2;

            List<CardData> result = new List<CardData>(total);
            List<CardData> variations = m_GameData.Cards;

            int variationCount = variations.Count;

            for (int i = 0; i < pairCount; i++)
            {
                CardData data = variations[i % variationCount];
                result.Add(data);
                result.Add(data);
            }

            Shuffle(result);

            return result;
        }

        private void Shuffle(List<CardData> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        private float CalculateCardScale(int width, int height)
        {
            float scaleX = (float)m_MaxGridSizeX / width;
            float scaleY = (float)m_MaxGridSizeY / height;

            return Mathf.Min(scaleX, scaleY) - m_Spacing;
        }

        private void ClearGrid()
        {
            for (int i = 0; i < m_ActiveCards.Count; i++)
                m_CardPool.Return(m_ActiveCards[i]);

            m_ActiveCards.Clear();
        }
    
        public List<Card> GetActiveCards()
        {
            return m_ActiveCards;
        }
    
        public GridData CreateSnapshot()
        {
            GridData save = new GridData();

            save.GridX = m_GridX;
            save.GridY = m_GridY;

            save.CardIds = new List<int>();
            save.MatchedState = new List<bool>();

            foreach (Card card in m_ActiveCards)
            {
                save.CardIds.Add(card.Id);
                save.MatchedState.Add(card.IsMatched);
            }

            return save;
        }

        public void LoadFromSnapshot(GridData saveData, Dictionary<int, CardData> cardsData, System.Action<Card> onSelected,
            out int totalPairs)
        {
            ClearGrid();

            m_GridX = saveData.GridX;
            m_GridY = saveData.GridY;

            BuildEmptyGrid();
            int remaingCards = 0;

            for (int i = 0; i < saveData.CardIds.Count; i++)
            {
                bool isMatched = saveData.MatchedState[i];
                int cardId = saveData.CardIds[i];

                Card card = m_ActiveCards[i];
                cardsData.TryGetValue(cardId, out CardData data);
                card.Initialize(data, onSelected);

                if (isMatched)
                    card.SetMatchedStateImmediate();
                else
                {
                    remaingCards++;
                }
            }

            totalPairs = remaingCards / 2;
        }

        private void BuildEmptyGrid()
        {
            ClearGrid();

            int total = m_GridX * m_GridY;
            m_CardPool.Prewarm(total);
        
            float cardScale = CalculateCardScale(m_GridX, m_GridY);

            Vector2 offset = new Vector2(
                (m_GridX - 1) * (cardScale + m_Spacing) * 0.5f,
                (m_GridY - 1) * (cardScale + m_Spacing) * 0.5f
            );

            for (int i = 0; i < total; i++)
            {
                Card card = m_CardPool.Get();
                m_ActiveCards.Add(card);

                card.transform.SetParent(m_BoardRoot);
                card.transform.localScale = Vector3.one * cardScale;

                int x = i % m_GridX;
                int y = i / m_GridX;

                card.transform.localPosition = new Vector3(
                    x * (cardScale + m_Spacing) - offset.x,
                    0,
                    -(y * (cardScale + m_Spacing) - offset.y)
                );
            }
        }
    }
}
