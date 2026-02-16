using DG.Tweening;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text m_ScoreText;
    [SerializeField] private TMP_Text m_ComboText;

    private int m_Score;
    private int m_ComboCounter;
    private int m_ScoreMultiplier;

    public void ResetScore()
    {
        m_Score = 0;
        m_ComboCounter = 0;
    }

    public void SetScoreMultiplier(int value)
    {
        m_ScoreMultiplier = value;
    }

    public void RegisterMatch()
    {
        m_ComboCounter++;
        int gained = m_ScoreMultiplier * m_ComboCounter;
        m_Score += gained;

        UpdateUI();
    }

    public void RegisterMiss()
    {
        m_ComboCounter = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_ScoreText.text = $"Score: {m_Score}";
        m_ComboText.text = m_ComboCounter > 1 ? $"Combo x{m_ComboCounter}" : "";
        
        m_ScoreText.transform
            .DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0.5f);

        if (m_ComboCounter > 1)
        {
            m_ComboText.gameObject.SetActive(true);
            
            m_ComboText.transform
                .DOPunchScale(Vector3.one * 0.3f, 0.2f);
        }
        else
        {
            m_ComboText.gameObject.SetActive(false);
        }
    }
}