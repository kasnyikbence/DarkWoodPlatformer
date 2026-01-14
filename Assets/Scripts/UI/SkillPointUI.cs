using System;
using TMPro;
using UnityEngine;

public class SkillPointUI : MonoBehaviour
{
    [Header("UI Referenciák")]
    public TextMeshProUGUI skillPointsText;
    public GameObject skillPointImage;

    private void Start()
    {
        if (SkillManager.Instance != null)
        {

            SkillManager.Instance.OnSkillTreeChanged += UpdateUI;

            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillTreeChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (SkillManager.Instance == null) return;

        int points = SkillManager.Instance.skillPoints;

        if (skillPointsText != null)
        {
            skillPointsText.text = points.ToString();
        }

        if (skillPointImage != null)
        {
            if (points > 0)
            {
                skillPointImage.SetActive(true);
            }
            else
            {
                skillPointImage.SetActive(false);
            }
        }
    }
}