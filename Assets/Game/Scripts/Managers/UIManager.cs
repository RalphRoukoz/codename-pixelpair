using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager m_GameManager;

    [SerializeField] private GameObject m_GridGeneratorPanel;
    [SerializeField] private GameObject m_MatchPanel;
    private void Awake()
    {
        m_GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        m_GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Setup:
                m_GridGeneratorPanel.SetActive(true);
                m_MatchPanel.SetActive(false);
                break;
            
            case GameState.Preview:
                m_GridGeneratorPanel.SetActive(false);
                m_MatchPanel.SetActive(false);
                break;
            
            case GameState.Playing:
                m_MatchPanel.SetActive(true);
                break;
            
            case GameState.Victory:
            case GameState.GameOver:
                m_GridGeneratorPanel.SetActive(true);
                m_MatchPanel.SetActive(false);
                break;
        }
    }
}
