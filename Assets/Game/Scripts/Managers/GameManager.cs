using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private readonly List<Card> selectedCards = new List<Card>(2);

    [SerializeField] private GridManager m_GridManager;
    [SerializeField] private float m_PreviewCardsDuration;
    
    private GameState m_CurrentState;

    public void StartGame(int gridX, int gridY)
    {
        m_CurrentState = GameState.Setup;
        
        m_GridManager.GenerateGrid(gridX, gridY, OnCardSelected);
        
        StartPreviewSequence();
    }
    
    private void StartPreviewSequence()
    {
        m_CurrentState = GameState.Preview;

        List<Card> cards = m_GridManager.GetActiveCards();

        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < cards.Count; i++)
        {
            sequence.Join(cards[i].FlipPreview(true));
        }

        sequence.AppendInterval(m_PreviewCardsDuration);

        for (int i = 0; i < cards.Count; i++)
        {
            sequence.Join(cards[i].FlipPreview(false));
        }

        sequence.OnComplete(() =>
        {
            m_CurrentState = GameState.Playing;
        });
    }
    
    private void OnCardSelected(Card card)
    {
        if (card.IsFlipped) return;

        card.Flip(true);

        selectedCards.Add(card);

        if (selectedCards.Count >= 2)
        {
            StartCoroutine(ProcessComparison());
        }
    }

    private IEnumerator ProcessComparison()
    {
        yield return new WaitForSeconds(0.5f);
        
        Card a = selectedCards[0];
        Card b = selectedCards[1];

        if (a != null && b != null)
        {
            if (a.Id == b.Id)
            {
                a.SetMatched();
                b.SetMatched();
            }
            else
            {
                a.Flip(false);
                b.Flip(false);
            }
            
            selectedCards.Clear();
        }
    }
}

public enum GameState
{
    Setup,
    Preview,
    Playing,
    GameOver
}