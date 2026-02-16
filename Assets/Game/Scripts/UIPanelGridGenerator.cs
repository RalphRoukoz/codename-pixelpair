using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelGridGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager m_GridManager;
    
    [Header("UI Elements")]
    [SerializeField] private Button m_GenerateButton;
    [SerializeField] private TextMeshProUGUI m_WarningText;
    [SerializeField] private TMP_InputField m_InputFieldY;
    [SerializeField] private TMP_InputField m_InputFieldX;
    
    private int m_GridX;
    private int m_GridY;

    private void Awake()
    {
        m_InputFieldX.onValueChanged.AddListener(OnInputFieldXValueChanged);
        m_InputFieldY.onValueChanged.AddListener(OnInputFieldYValueChanged);

        m_GenerateButton.onClick.AddListener(OnGenerateButtonPressed);
    }

    private void OnDestroy()
    {
        m_InputFieldX.onValueChanged.RemoveListener(OnInputFieldXValueChanged);
        m_InputFieldY.onValueChanged.RemoveListener(OnInputFieldYValueChanged);

        m_GenerateButton.onClick.RemoveListener(OnGenerateButtonPressed);
    }

    private void OnGenerateButtonPressed()
    {
        m_GridManager.GenerateGrid(m_GridX, m_GridY);
        gameObject.SetActive(false);
    }

    private void OnInputFieldXValueChanged(string gridX)
    {
        if(!int.TryParse(gridX, out int x)) return;

        m_GridX = x;
        
        CheckForWarning();
    }
    
    private void OnInputFieldYValueChanged(string gridY)
    {
        if(!int.TryParse(gridY, out int y)) return;

        m_GridY = y;
        
        CheckForWarning();
    }

    private void CheckForWarning()
    {
        m_WarningText.gameObject.SetActive(true);
        
        if (m_GridX == 0 || m_GridY == 0)
        {
            m_WarningText.text = "Grid X and Y must be greater than zero.";
            m_GenerateButton.interactable = false;
            return;
        }
        
        int total = m_GridX * m_GridY;

        if (total % 2 != 0)
        {
            m_WarningText.text = "Grid must be even.";
            m_GenerateButton.interactable = false;
            return;
        }
        
        m_WarningText.gameObject.SetActive(false);
        m_GenerateButton.interactable = true;
    }
}
