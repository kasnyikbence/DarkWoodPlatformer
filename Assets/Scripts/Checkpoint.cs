using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    private bool playerInRange = false;
    private GameObject player;
    public string saveName = "savedGame";
    public string directoryName = "Saves";
    public SaveGameData saveGameData;
    private PlayerInput playerInput;


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player = col.gameObject;
            playerInput = player.GetComponent<PlayerInput>();

            playerInRange = true;
            if (playerInput != null)
            {
                playerInput.actions["CheckpointSave"].performed -= OnInteract;
                playerInput.actions["CheckpointSave"].performed += OnInteract;
            }
            UIManager.Instance.ShowInteractHint();
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = false;
            if (playerInput != null)
            {
                playerInput.actions["CheckpointSave"].performed -= OnInteract;
            }
            UIManager.Instance.HideInteractHint();
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange)
        {
            SaveCheckpoint();
        }
    }
    private void SaveCheckpoint()
    {
        saveGameData.health = player.GetComponent<Damageable>().Health;

        PotionSystem potionSystem = player.GetComponent<PotionSystem>();
        ProjectileLauncher projectileLauncher = player.GetComponent<ProjectileLauncher>();

        if (potionSystem != null)
        {
            saveGameData.potionAmount = potionSystem.currentPotions;
        }
        if (projectileLauncher != null)
        {
            saveGameData.arrowAmount = projectileLauncher.currentArrows; 
        }

        Vector3 playerPos = player.transform.position;
        saveGameData.playerPositionX = playerPos.x;
        saveGameData.playerPositionY = playerPos.y;
        saveGameData.playerPositionZ = playerPos.z;

        saveGameData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (ExperienceManager.Instance != null)
        {
            saveGameData.currentLevel = ExperienceManager.Instance.CurrentLevel;
            saveGameData.totalExperience = ExperienceManager.Instance.TotalExperience;
        }

        if (SkillManager.Instance != null)
        {
            saveGameData.skillPoints = SkillManager.Instance.skillPoints;
            saveGameData.unlockedSkillIDs = SkillManager.Instance.unlockedSkilIDs;
        }

        if (PlayerStats.Instance != null)
        {
            saveGameData.damageMultiplier = PlayerStats.Instance.damageMultiplier;
            saveGameData.rangedDamageMultiplier = PlayerStats.Instance.rangedDamageMultiplier;
            saveGameData.critChance = PlayerStats.Instance.critChance;
            saveGameData.lifeStealAmount = PlayerStats.Instance.lifeStealAmount;

            saveGameData.bonusMaxHealth = PlayerStats.Instance.bonusMaxHealth;
            saveGameData.bonusMaxPotions = PlayerStats.Instance.bonusMaxPotions;
            saveGameData.bonusMaxArrows = PlayerStats.Instance.bonusMaxArrows;

            saveGameData.doubleJumpUnlocked = PlayerStats.Instance.doubleJumpUnlocked;
        }

        if (GameManager.Instance != null)
        {
            saveGameData.openedChestIDs = new List<string>(GameManager.Instance.openedChests);
            saveGameData.deadEnemyIDs = new List<string>(GameManager.Instance.deadEnemies);
        }
        else
        {
            saveGameData.openedChestIDs = new List<string>();
            saveGameData.deadEnemyIDs = new List<string>();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameSavedText(transform.position);
        }
        string savePath = Application.persistentDataPath + "/" + directoryName;
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create(savePath + "/" + saveName + ".bin");

        formatter.Serialize(saveFile, saveGameData);
        saveFile.Close();

        print("Játék mentve: " + savePath + "/" + saveName + ".bin");
    }
}