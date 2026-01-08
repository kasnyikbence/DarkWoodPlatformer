using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    int totalExperience;
    int previousLevelExp, nextLevelExp;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image expFill;

    public int CurrentLevel { get; private set; } = 1;


    void Start()
    {
        UpdateLevel();
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    void CheckForLevelUp()
    {
        if (totalExperience >= nextLevelExp)
        {
            CurrentLevel++;
            if (SkillManager.Instance != null)
            {
                SkillManager.Instance.AddSkillPoints(1);
            }

            UpdateLevel();
        }
    }

    void UpdateLevel()
    {
        previousLevelExp = (int)experienceCurve.Evaluate(CurrentLevel);
        nextLevelExp = (int)experienceCurve.Evaluate(CurrentLevel + 1);
        UpdateInterface();
    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelExp;
        int end = nextLevelExp - previousLevelExp;

        levelText.text = CurrentLevel.ToString();
        experienceText.text = start + " exp / " + end + " exp";

        if (expFill != null && end > 0)
        {
            expFill.fillAmount = (float)start / (float)end;
        }
    }
}
