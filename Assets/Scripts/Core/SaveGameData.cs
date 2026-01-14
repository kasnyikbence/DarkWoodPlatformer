using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SaveGameData
{
    //Character info
    public int health;
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;
    public int potionAmount;
    public int arrowAmount;

    //Scene
    public int sceneIndex;

    //XP & Skill
    public int currentLevel;
    public int totalExperience;
    public int skillPoints;
    public List<int> unlockedSkillIDs;
    public float damageMultiplier;
    public float rangedDamageMultiplier;
    public float critChance;
    public float lifeStealAmount;
    public int bonusMaxHealth;
    public int bonusMaxPotions;
    public int bonusMaxArrows;
    public bool doubleJumpUnlocked;

    //Save Slot
    //public string lastPlayedDate;
    //public float playTimeSeconds;
}