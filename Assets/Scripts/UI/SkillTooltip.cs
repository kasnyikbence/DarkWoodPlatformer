using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillTooltip : MonoBehaviour
{
    public static SkillTooltip Instance;

    [Header("UI Referenciák")]
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI descreptionField;

    public LayoutElement layoutElement;

    [Header("Beállítások")]
    public int characterWrapLimit = 80;

    private RectTransform rectTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rectTransform = GetComponent<RectTransform>();

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        gameObject.SetActive(false);
    }

    public void ShowTooltip(string header, string content)
    {
        if (string.IsNullOrEmpty(header) && string.IsNullOrEmpty(content)) return;
        gameObject.SetActive(true);

        headerField.text = header;
        descreptionField.text = content;

        int headerLength = headerField.text.Length;
        int descreptionLength = descreptionField.text.Length;

        if (layoutElement != null)
        {
            layoutElement.enabled = (headerLength > characterWrapLimit || descreptionLength > characterWrapLimit);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}