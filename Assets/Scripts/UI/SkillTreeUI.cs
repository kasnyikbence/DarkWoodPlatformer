using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeUI : MonoBehaviour
{
    public static bool isOpen = false;
    public GameObject skillTreePanel;
    public TextMeshProUGUI skillPointsText;

    void Start()
    {
        if (skillTreePanel != null)
        {
            skillTreePanel.SetActive(false);
        }
        isOpen = false;

        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillTreeChanged += UpdateSkillPointText;

            UpdateSkillPointText();
        }
    }
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleSkillTree();
        }
    }

    private void OnDestroy()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillTreeChanged -= UpdateSkillPointText;
        }
    }

    public void ToggleSkillTree()
    {
        if (skillTreePanel == null) return;

        isOpen = !isOpen;

        if (isOpen)
        {
            skillTreePanel.SetActive(true);
            Time.timeScale = 0f;
            UpdateSkillPointText();

            if (UIManager.Instance != null) UIManager.Instance.HideInteractHint();
        }
        else
        {
            skillTreePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void UpdateSkillPointText()
    {
        if (skillPointsText != null && SkillManager.Instance != null)
        {
            skillPointsText.text = "Skill Points: " + SkillManager.Instance.skillPoints.ToString();
        }
    }
}
