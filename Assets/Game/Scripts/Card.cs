using DG.Tweening;
using Game.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class Card : MonoBehaviour
    {
        public int Id { get; private set; }
        public bool IsFlipped { get; private set; }
        public bool IsMatched { get; private set; }

        [SerializeField] private Image m_CardImage;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Transform m_CardTransform;
    
        private Tween m_FlipTween;
        private System.Action<Card> m_OnSelected;
    
        private static readonly int m_AnimFlipped = Animator.StringToHash("Flipped");
        private static readonly int m_AnimMatched = Animator.StringToHash("Matched");

        public void Initialize(CardData data, System.Action<Card> onSelected)
        {
            Id = data.ID;
            m_CardImage.sprite = data.Sprite;
            m_OnSelected = onSelected;
        
            ResetState();
        }

        public void Select()
        {
            if (IsFlipped || IsMatched)
                return;

            m_OnSelected?.Invoke(this);
        }

        public void Flip(bool showFront)
        {
            IsFlipped = showFront;
            m_Animator?.SetBool(m_AnimFlipped, IsFlipped);

            m_FlipTween = m_CardTransform
                .DORotate(new Vector3(showFront ? 0f : 180f, 0, 0), 0.25f)
                .SetEase(Ease.OutQuad);
        }

        public void SetMatched()
        {
            IsMatched = true;
            m_Animator?.SetTrigger(m_AnimMatched);
        }

        public void ResetState()
        {
            if (m_FlipTween != null && m_FlipTween.IsActive())
                m_FlipTween.Kill();
        
            m_CardTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            IsFlipped = false;
            IsMatched = false;
            m_Animator?.SetBool(m_AnimFlipped, IsFlipped);
        }
    
        public Tween FlipPreview(bool showFront)
        {
            IsFlipped = showFront;

            return m_CardTransform
                .DORotate(new Vector3(showFront ? 0f : 180f, 0, 0), 0.25f)
                .SetEase(Ease.OutQuad);
        }
    
        public void SetMatchedStateImmediate()
        {
            IsMatched = true;
            IsFlipped = true;
            gameObject.SetActive(false);
        }

    }
}