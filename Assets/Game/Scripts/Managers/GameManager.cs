using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        private readonly List<Card> m_SelectedCards = new List<Card>(2);
        private readonly Queue<(Card, Card)> m_ComparisonQueue = new Queue<(Card, Card)>();
    
        [Header("Manager References")]
        [SerializeField] private GridManager m_GridManager;
        [SerializeField] private ScoreManager m_ScoreManager;
        [SerializeField] private TimerManager m_TimerManager;
        [SerializeField] private AudioManager m_AudioManager;
    
        [SerializeField] private GameData m_GameData;
        [SerializeField] private float m_PreviewCardsDuration;
    
        private GameState m_CurrentState;
        private int m_TotalPairs;
        private int m_MatchedPairs;
        private Dictionary<int, CardData> m_CardLookup = new Dictionary<int, CardData>();
        private MatchSaveData m_LastMatchSaveData;
        
        public System.Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            m_TimerManager.OnTimerEnd += HandleTimeUp;

            foreach (CardData card in m_GameData.Cards)
            {
                if (!m_CardLookup.TryAdd(card.ID, card))
                    Debug.LogWarning($"Duplicate CardData Id detected: {card.ID}");
            }
        }

        private void Start()
        {
            MatchSaveData save = SaveLoadSystem.Load();

            if (save != null)
            {
                m_LastMatchSaveData = save;
                LoadGame();
            }
            else
            {
                SetGameState(GameState.Setup);
            }

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
    
        private void StartPreviewSequence(bool loadedMatch = false)
        {
            SetGameState(GameState.Preview);
        
            List<Card> cards = m_GridManager.GetActiveCards();

            Sequence previewSequence = DOTween.Sequence();
            Sequence flipDownSequence = DOTween.Sequence();
            flipDownSequence.OnStart(() => {m_AudioManager.PlayFlip();});
            for (int i = 0; i < cards.Count; i++)
            {
                flipDownSequence.Join(cards[i].FlipPreview(false));
            }
        
            previewSequence
                .AppendInterval(m_PreviewCardsDuration)
                .Append(flipDownSequence)
                .OnComplete(() => { OnPreviewComplete(loadedMatch); });
        }

        private void OnPreviewComplete(bool loadedMatch)
        {
            SetGameState(GameState.Playing);
            m_TimerManager.StartTimer(loadedMatch? m_LastMatchSaveData.TimeRemaining : m_GameData.MatchDuration);
        }

        private void OnCardSelected(Card card)
        {
            if (m_CurrentState != GameState.Playing)
                return;
        
            if (card.IsFlipped) return;

            card.Flip(true);
            m_AudioManager.PlayFlip();
        
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
                        StartCoroutine(HandleVictory());
                    }
                    m_AudioManager.PlayMatch();
                }
                else
                {
                    a.Flip(false);
                    b.Flip(false);
                    m_ScoreManager.RegisterMiss();
                    m_AudioManager.PlayMismatch();
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

            m_AudioManager.PlayGameOver();
            SetGameState(GameState.GameOver);
        }

        private IEnumerator HandleVictory()
        {
            m_TimerManager.StopTimer();
            SaveLoadSystem.ClearSave();
            
            yield return new WaitForSeconds(0.5f);
            
            SetGameState(GameState.Victory);
            m_ScoreManager.ResetScore();
        }
    
        private void ProcessQueue()
        {
            if (m_ComparisonQueue.Count == 0)
                return;

            (Card, Card) pair = m_ComparisonQueue.Dequeue();
        
            StartCoroutine(ProcessComparison(pair.Item1, pair.Item2));
        }
    
        private void SaveGame()
        {
            MatchSaveData snapshot = new MatchSaveData()
            {
                GridData = m_GridManager.CreateSnapshot(),
                Score = m_ScoreManager.Score,
                TimeRemaining = m_TimerManager.TimeRemaining,
            };

            SaveLoadSystem.Save(snapshot);
        }
    
        private void OnApplicationPause(bool pause)
        {
            if (pause && m_CurrentState == GameState.Playing)
                SaveGame();
        }

        private void OnApplicationQuit()
        {
            if (m_CurrentState == GameState.Playing)
                SaveGame();
        }

        private void LoadGame()
        {
            m_GridManager.LoadFromSnapshot(m_LastMatchSaveData.GridData, m_CardLookup, OnCardSelected, out m_TotalPairs);
            
            StartPreviewSequence(true);

            m_TimerManager.StartTimer(m_LastMatchSaveData.TimeRemaining);
            m_ScoreManager.SetScore(m_LastMatchSaveData.Score);
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
}