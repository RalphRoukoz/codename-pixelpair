using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private readonly List<Card> m_SelectedCards = new List<Card>(2);
    private readonly Queue<(Card, Card)> m_ComparisonQueue = new Queue<(Card, Card)>();
    
    [Header("Manager References")]
    [SerializeField] private GridManager m_GridManager;
    [SerializeField] private ScoreManager m_ScoreManager;
    [SerializeField] private TimerManager m_TimerManager;
    
    [SerializeField] private GameData m_GameData;
    
    [SerializeField] private float m_PreviewCardsDuration;
    
    private GameState m_CurrentState;
    private int m_TotalPairs;
    private int m_MatchedPairs;
    
    public System.Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        m_TimerManager.OnTimerEnd += HandleTimeUp;
    }

    private void Start()
    {
        SetGameState(GameState.Setup);
        m_ScoreManager.SetScoreMultiplier(m_GameData.ScoreMultiplier);
    }

    private void OnDestroy()
    {
        m_TimerManager.OnTimerEnd -= HandleTimeUp;
    }

    public void StartGame(int gridX, int gridY)
    {
        m_MatchedPairs = 0;
        
        m_GridManager.GenerateGrid(gridX, gridY, OnCardSelected, out m_TotalPairs);
        
        StartPreviewSequence();
    }
    
    private void StartPreviewSequence()
    {
        SetGameState(GameState.Preview);
        
        List<Card> cards = m_GridManager.GetActiveCards();

        Sequence previewSequence = DOTween.Sequence();

        Sequence flipUpSequence = DOTween.Sequence();
        for (int i = 0; i < cards.Count; i++)
        {
            flipUpSequence.Join(cards[i].FlipPreview(true));
        }

        Sequence flipDownSequence = DOTween.Sequence();
        for (int i = 0; i < cards.Count; i++)
        {
            flipDownSequence.Join(cards[i].FlipPreview(false));
        }

        previewSequence
            .Append(flipUpSequence)
            .AppendInterval(m_PreviewCardsDuration)
            .Append(flipDownSequence)
            .OnComplete(() =>
            {
                SetGameState(GameState.Playing);
                m_TimerManager.StartTimer(m_GameData.MatchDuration);
            });
    }
    
    private void OnCardSelected(Card card)
    {
        if (m_CurrentState != GameState.Playing)
            return;
        
        if (card.IsFlipped) return;

        card.Flip(true);

        m_SelectedCards.Add(card);

        if (m_SelectedCards.Count >= 2)
        {
            m_ComparisonQueue.Enqueue((m_SelectedCards[0], m_SelectedCards[1]));
            m_SelectedCards.Clear();

            ProcessQueue();
        }
    }

    private IEnumerator ProcessComparison(Card a, Card b)
    {
        yield return new WaitForSeconds(0.5f);

        if (a != null && b != null)
        {
            if (a.Id == b.Id)
            {
                a.SetMatched();
                b.SetMatched();
                m_MatchedPairs++;
                m_ScoreManager.RegisterMatch();
                
                if (m_MatchedPairs >= m_TotalPairs)
                {
                    HandleVictory();
                }
            }
            else
            {
                a.Flip(false);
                b.Flip(false);
                m_ScoreManager.RegisterMiss();
            }

            ProcessQueue();
        }
    }
    
    private void SetGameState(GameState state)
    {
        m_CurrentState = state;
        OnGameStateChanged?.Invoke(m_CurrentState);
    }
    
    private void HandleTimeUp()
    {
        if (m_CurrentState != GameState.Playing)
            return;

        SetGameState(GameState.GameOver);
    }

    private void HandleVictory()
    {
        SetGameState(GameState.Victory);
        m_TimerManager.StopTimer();
        m_ScoreManager.ResetScore();
    }
    
    private void ProcessQueue()
    {
        if (m_ComparisonQueue.Count == 0)
            return;

        (Card, Card) pair = m_ComparisonQueue.Dequeue();
        
        StartCoroutine(ProcessComparison(pair.Item1, pair.Item2));
    }
}

public enum GameState
{
    Setup,
    Preview,
    Playing,
    Victory,
    GameOver
}