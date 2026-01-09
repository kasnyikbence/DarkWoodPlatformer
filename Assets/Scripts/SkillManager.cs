using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
   public static SkillManager Instance;

    public int skillPoints = 0;
    public List<int> unlockedSkilIDs = new List<int>();

    public event Action OnSkillTreeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (skillPoints == 0)
        {
           // skillPoints = 5; Teszt
            OnSkillTreeChanged?.Invoke();
        }
    }


    public bool IsSkillUnlocked(int skillID)
    {
        return unlockedSkilIDs.Contains(skillID);
    }

    public void TryUnlockSkill(SkillSlot slot)
    {
        if(IsSkillUnlocked(slot.skillID))
        {
            Debug.Log("Ez a skill már megvan!");
            return;
        }

        if (skillPoints < slot.cost)
        {
            Debug.Log("Nincs elég skill point!");
            return;
        }

        if (!CheckParents(slot))
        {
            Debug.Log("Elõbb az elõzõ képességet kell feloldanod!");
            return;
        }

        skillPoints -= slot.cost;
        unlockedSkilIDs.Add(slot.skillID);

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.UnlockBonus(slot.skillType);
        }

        OnSkillTreeChanged?.Invoke();

    }

    private bool CheckParents(SkillSlot slot)
    {

        if (slot.parentSkills == null || slot.parentSkills.Length == 0)
        {
            return true;
        }

        foreach (SkillSlot parent in slot.parentSkills)
        {
            if (!IsSkillUnlocked(parent.skillID))
            {
                return false;
            }
        }
        return true;
    }

    public void AddSkillPoints(int amount) 
    { 
        skillPoints += amount; 
        OnSkillTreeChanged?.Invoke(); 
    }

    public void LoadSkills(int points, List<int> unlockedIds)
    {
        skillPoints = points;

        if (unlockedIds != null)
        {
            unlockedSkilIDs = new List<int>(unlockedIds);
        }
        else
        {
            unlockedSkilIDs = new List<int>();
        }

        OnSkillTreeChanged?.Invoke();
    }
}
