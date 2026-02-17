using TMPro;
using UnityEngine;

namespace Game.Scripts.Managers
{
    public class TimerManager : MonoBehaviour
    {
        public float TimeRemaining => m_TimeRemaining;
        
        [SerializeField] private TMP_Text m_TimerText;

        private float m_TimeRemaining;
        private bool m_IsRunning;

        public System.Action OnTimerEnd;

        public void StartTimer(float duration)
        {
            m_TimeRemaining = duration;
            m_IsRunning = true;
            UpdateUI();
        }

        public void StopTimer()
        {
            m_IsRunning = false;
        }

        private void Update()
        {
            if (!m_IsRunning) return;

            m_TimeRemaining -= Time.deltaTime;

            if (m_TimeRemaining <= 0f)
            {
                m_TimeRemaining = 0f;
                m_IsRunning = false;
                UpdateUI();
                OnTimerEnd?.Invoke();
                return;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            int seconds = Mathf.CeilToInt(m_TimeRemaining);
            m_TimerText.text = seconds.ToString();
        }
    }
}