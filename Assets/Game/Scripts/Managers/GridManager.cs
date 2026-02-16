using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform m_BoardRoot;
    [SerializeField] private CardPoolManager m_CardPool;
    [SerializeField] private GameData m_GameData;
    
    [Header("Grid Settings")]
    [SerializeField] private int m_MaxGridSizeX = 100;
    [SerializeField] private int m_MaxGridSizeY = 100;
    [SerializeField] private float m_Spacing = 0.2f;
    
    private readonly List<Card> m_ActiveCards = new List<Card>();

    public void GenerateGrid(int gridX, int gridY, System.Action<Card> onSelected, out int totalPairs)
    {
        ClearGrid();

        int total = gridX * gridY;
        totalPairs = total / 2;
        
        m_CardPool.Prewarm(total);

        List<CardData> generatedSet = GenerateCardSet(total);

        float cardScale = CalculateCardScale(gridX, gridY);

        Vector2 offset = new Vector2(
            (gridX - 1) * (cardScale + m_Spacing) * 0.5f,
            (gridY - 1) * (cardScale + m_Spacing) * 0.5f
        );

        for (int i = 0; i < total; i++)
        {
            Card card = m_CardPool.Get();
            m_ActiveCards.Add(card);

            card.transform.SetParent(m_BoardRoot);
            card.transform.localScale = Vector3.one * cardScale;

            int x = i % gridX;
            int y = i / gridX;

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
}
