using UnityEngine;
using System.Collections.Generic;

public class CardPoolManager : MonoBehaviour
{
    [SerializeField] private Card m_CardPrefab;

    private readonly Stack<Card> m_Pool = new Stack<Card>();
    private readonly List<Card> m_AllCreated = new List<Card>();

    public Card Get()
    {
        if (m_Pool.Count > 0)
        {
            Card reused = m_Pool.Pop();
            reused.gameObject.SetActive(true);
            return reused;
        }

        Card created = Instantiate(m_CardPrefab, transform);
        m_AllCreated.Add(created);
        return created;
    }

    public void Return(Card card)
    {
        card.ResetState();
        card.gameObject.SetActive(false);
        m_Pool.Push(card);
    }

    public void Prewarm(int amount)
    {
        int needed = amount - m_AllCreated.Count;

        for (int i = 0; i < needed; i++)
        {
            Card card = Instantiate(m_CardPrefab, transform);
            card.gameObject.SetActive(false);
            m_Pool.Push(card);
            m_AllCreated.Add(card);
        }
    }
}