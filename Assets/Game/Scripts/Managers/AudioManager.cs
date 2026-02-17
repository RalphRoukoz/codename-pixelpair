using UnityEngine;

namespace Game.Scripts.Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource m_Source;
        [SerializeField] private AudioClip m_Flip;
        [SerializeField] private AudioClip m_Match;
        [SerializeField] private AudioClip m_Mismatch;
        [SerializeField] private AudioClip m_GameOver;

        public void PlayFlip() => m_Source.PlayOneShot(m_Flip);
        public void PlayMatch() => m_Source.PlayOneShot(m_Match);
        public void PlayMismatch() => m_Source.PlayOneShot(m_Mismatch);
        public void PlayGameOver() => m_Source.PlayOneShot(m_GameOver);
    }
}