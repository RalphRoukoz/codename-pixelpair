using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public int Score => m_Score;
    
        [SerializeField] private TMP_Text m_ScoreText;
        [SerializeField] private TMP_Text m_ScoreGainedText;
        
        private int m_Score;
        private int m_ComboCounter;
        private int m_ScoreMultiplier;
        private float m_FadeOutDelay = 1f;
        
        private Tween m_FadeTween;
        private Tween m_PunchTween;

        private void Start()
        {
            m_ScoreGainedText.alpha = 0;
        }

        public void ResetScore()
        {
            m_Score = 0;
            m_ComboCounter = 0;
            m_ScoreText.text = $"Score: {m_Score}";
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
        }

        private void UpdateUI()
        {
            m_ScoreText.text = $"Score: {m_Score}";

            m_ScoreGainedText.text = m_ComboCounter > 1 ? $"{m_ComboCounter} x {m_ScoreMultiplier}" : $"{m_ScoreMultiplier}";

            if (m_ScoreGainedText.color.a < 0.01f)
            {
                m_ScoreGainedText.DOFade(1f, 0.2f).SetUpdate(true);
            }

            m_PunchTween?.Kill();
            m_PunchTween = m_ScoreGainedText.transform
                .DOPunchScale(Vector3.one * 0.3f, 0.3f)
                .SetUpdate(true);
            
            m_FadeTween?.Kill();
            m_FadeTween = m_ScoreGainedText.DOFade(0f, 0.2f)
                .SetDelay(m_FadeOutDelay) 
                .SetUpdate(true);
        }
    
        public void SetScore(int score)
        {
            m_Score = score;
            m_ScoreText.text = $"Score: {m_Score}";
        }
    }
}