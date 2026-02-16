using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int Id { get; private set; }
    public bool IsFlipped { get; private set; }

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
        if (IsFlipped)
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
        m_Animator?.SetTrigger(m_AnimMatched);
    }

    public void ResetState()
    {
        if (m_FlipTween != null && m_FlipTween.IsActive())
            m_FlipTween.Kill();
        
        IsFlipped = false;
        m_Animator?.SetBool(m_AnimFlipped, IsFlipped);
    }
    
    public Tween FlipPreview(bool showFront)
    {
        IsFlipped = showFront;

        return m_CardTransform
            .DORotate(new Vector3(showFront ? 0f : 180f, 0, 0), 0.25f)
            .SetEase(Ease.OutQuad);
    }
}