using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlot : MonoBehaviour
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
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color notOwnedColor = Color.gray;
    public Color ownedColor = Color.white;

    public SkillSlot[] parentSkills;

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

            // Azonnali frissítés induláskor
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

        if (SkillManager.Instance.IsSkillUnlocked(skillID))
        {
            UpdateUIState(SkillState.Unlocked);
            return;
        }

        if (AreParentsUnlocked())
        {
            UpdateUIState(SkillState.Unlockable);
        }
        else
        {
            UpdateUIState(SkillState.Locked);

        }
    }
    private bool AreParentsUnlocked()
    {
        if (parentSkills == null || parentSkills.Length == 0) return true; // Tier 1

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
}

public enum SkillState
{
    Locked,
    Unlockable,
    Unlocked
}