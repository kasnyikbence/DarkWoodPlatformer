using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Kell a Tooltiphez

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Skill Adatok")]
    public int skillID;
    public SkillType skillType;
    public string skillName;
    public int cost = 1;
    [TextArea] public string description;

    [Header("UI Referenciák")]
    public Button button;
    public Image iconImage;
    private Image frameImage;

    [Header("Színek")]
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Sötétszürke (Zárva)
    public Color notOwnedColor = Color.gray;                    // Világosszürke (Megvehetõ)
    public Color ownedColor = Color.white;                      // Fehér (Megvan)

    public SkillSlot[] parentSkills;

    // --- ÁLLAPOTOK ---
    private SkillState currentState;
    private bool isHovered = false; // ÚJ: Tudjuk, hogy rajta van-e az egér

    void Awake()
    {
        if (button != null)
        {
            frameImage = button.GetComponent<Image>();
        }
        else
        {
            frameImage = GetComponent<Image>();
            button = GetComponent<Button>();
        }
    }

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnSkillClicked);
        }

        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillTreeChanged += UpdateUI;
            UpdateUI();
        }
    }

    void OnDestroy()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillTreeChanged -= UpdateUI;
        }
    }

    public void UpdateUIState(SkillState state)
    {
        currentState = state;

        if (frameImage == null && button != null) frameImage = button.GetComponent<Image>();

        switch (state)
        {
            case SkillState.Locked:
                if (button) button.interactable = false;
                SetColor(lockedColor);
                break;

            case SkillState.Unlockable:
                if (button) button.interactable = true;
                SetColor(notOwnedColor);
                break;

            case SkillState.Unlocked:
                if (button) button.interactable = false;
                SetColor(ownedColor);
                break;
        }
    }

    public void UpdateUI()
    {
        if (SkillManager.Instance == null) return;

        // 1. Állapot frissítése
        if (SkillManager.Instance.IsSkillUnlocked(skillID))
        {
            UpdateUIState(SkillState.Unlocked);
        }
        else if (AreParentsUnlocked())
        {
            UpdateUIState(SkillState.Unlockable);
        }
        else
        {
            UpdateUIState(SkillState.Locked);
        }

        // 2. ÚJ: Ha épp rajta van az egér, frissítjük a Tooltipet is azonnal!
        // Így kattintás után rögtön átvált a szöveg [Owned]-re.
        if (isHovered)
        {
            ShowTooltipContent();
        }
    }

    private bool AreParentsUnlocked()
    {
        if (parentSkills == null || parentSkills.Length == 0) return true;

        foreach (var parent in parentSkills)
        {
            if (SkillManager.Instance == null || !SkillManager.Instance.IsSkillUnlocked(parent.skillID))
            {
                return false;
            }
        }
        return true;
    }

    private void SetColor(Color color)
    {
        if (frameImage != null) frameImage.color = color;
        if (iconImage != null) iconImage.color = color;
    }

    public void OnSkillClicked()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.TryUnlockSkill(this);
        }
    }

    // --- TOOLTIP LOGIKA ---

    // Kiemeltem egy külön függvénybe, hogy többször is meg lehessen hívni
    private void ShowTooltipContent()
    {
        if (SkillTooltip.Instance != null)
        {
            string header = $"{skillName} (Cost: {cost})";

            if (currentState == SkillState.Locked)
            {
                header = $"{skillName} <color=red>[Locked]</color>";
            }
            else if (currentState == SkillState.Unlocked)
            {
                header = $"{skillName} <color=green>[Owned]</color>";
            }

            SkillTooltip.Instance.ShowTooltip(header, description);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        ShowTooltipContent();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (SkillTooltip.Instance != null)
        {
            SkillTooltip.Instance.HideTooltip();
        }
    }
}

public enum SkillState
{
    Locked,
    Unlockable,
    Unlocked
}