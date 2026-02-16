using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int Id { get; private set; }

    [SerializeField] private Image m_CardImage;

    public void Initialize(CardData data)
    {
        Id = data.ID;
        m_CardImage.sprite = data.Sprite;
    }
}