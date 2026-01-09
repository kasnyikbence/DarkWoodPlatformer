using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Combat")]
    public float damageMultiplier = 1.0f;
    public float rangedDamageMultiplier = 1.0f;
    public float critChance = 0.0f;
    public float critMultiplier = 2f;
    public float lifeStealAmount = 0f;

    [Header("Survival")]
    public int bonusMaxHealth = 0;
    public int bonusMaxPotions = 0;
    public int bonusMaxArrows = 0;

    [Header("Skill")]
    public bool doubleJumpUnlocked = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UnlockBonus(SkillType type)
    {
        switch (type)
        {
            //Tier 1
            case SkillType.Vitality1:
                bonusMaxHealth += 10;
                UpdateHealthComponent();
                break;
            case SkillType.Warrior1:
                damageMultiplier += 0.1f;
                break;

            //Tier 2
            case SkillType.DoubleJump:
                doubleJumpUnlocked = true;
                break;
            case SkillType.Alchemist1:
                bonusMaxPotions += 1;
                break;
            case SkillType.Hunter:
                bonusMaxArrows += 3;
                break;
            case SkillType.Vitality2:
                bonusMaxHealth += 15;
                UpdateHealthComponent();
                break;

            //Tier 3
            case SkillType.Alchemist2:
                bonusMaxPotions += 1;
                break;
            case SkillType.Sharpshooter:
                rangedDamageMultiplier += 0.2f;
                break;
            case SkillType.Warrior2:
                damageMultiplier += 0.15f;
                break;

            //Tier 4
            case SkillType.LifeSteal:
                lifeStealAmount += 3;
                break;
            case SkillType.CriticalStrike:
                critChance += 0.1f;
                break;
        }

        Debug.Log($"[PlayerStats] Bónusz aktiválva: {type}");
    }

    private void UpdateHealthComponent()
    {
        Damageable dmg = GetComponent<Damageable>();
        if (dmg != null)
        {
            dmg.UpdateMaxHealthUI();
            Debug.Log($"Új Max HP bónusz érvényesítve: +{bonusMaxHealth}");
        }
    }
}



public enum SkillType
{
    None,
    Vitality1,
    Warrior1,
    DoubleJump,
    Alchemist1,
    Hunter,
    Vitality2,
    Alchemist2,
    Sharpshooter,
    Warrior2,
    LifeSteal,
    CriticalStrike
}
