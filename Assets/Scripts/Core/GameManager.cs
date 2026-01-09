using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public int gameStartScene = 1;
    public string saveName = "savedGame";
    public string directoryName = "Saves";

    private SaveGameData? pendingLoadData = null;
    private GameObject player;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ---------------- scene load handling ----------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene loaded: {scene.name} (buildIndex {scene.buildIndex}).");

        // FŐMENÜ KEZELÉS
        if (scene.buildIndex == 0)
        {
            // Megpróbáljuk megtalálni a játékost, ha nincs meg
            if (player == null) player = GameObject.FindGameObjectWithTag("Player");

            // Ha megvan, elrejtjük
            if (player != null)
            {
                player.SetActive(false);
            }
            return;
        }

        // JÁTÉKMENET PÁLYA BETÖLTÉSE
        StartCoroutine(HandleSceneLoadedCoroutine());
    }

    private IEnumerator HandleSceneLoadedCoroutine()
    {
        yield return null;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                var allDamageables = Resources.FindObjectsOfTypeAll<Damageable>();
                foreach (var d in allDamageables)
                {
                    if (d.gameObject.scene.IsValid() && d.gameObject.CompareTag("Player"))
                    {
                        player = d.gameObject;
                        break;
                    }
                }
            }
        }

        if (player != null)
        {
            if (!player.activeInHierarchy)
            {
                player.SetActive(true);
            }

            var dmg = player.GetComponent<Damageable>();
            if (dmg != null)
            {
                dmg.OnPlayerDied -= HandleDeath;
                dmg.OnPlayerDied += HandleDeath;
            }
        }
        else
        {
            Debug.LogError("[GameManager] CRITICAL: Player not found anywhere!");
        }

        if (pendingLoadData.HasValue)
        {
            SaveGameData data = pendingLoadData.Value;
            pendingLoadData = null;

            yield return null;

            ApplyLoadData(data);
        }
    }

    // ---------------- delete save ----------------
    public void DeleteSaveData()
    {
        string savePath = Application.persistentDataPath + "/" + directoryName;
        string filePath = savePath + "/" + saveName + ".bin";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("[GameManager] Save file deleted: " + filePath);
        }
        else
        {
            Debug.Log("[GameManager] No save file to delete at: " + filePath);
        }
    }

    // ---------------- start / load game ----------------
    public void StartGame()
    {
        Time.timeScale = 1f;
        string path = Application.persistentDataPath + "/" + directoryName + "/" + saveName + ".bin";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream file = File.Open(path, FileMode.Open))
            {
                try
                {
                    SaveGameData data = (SaveGameData)formatter.Deserialize(file);
                    Debug.Log("[GameManager] Save found. Loading scene index: " + data.sceneIndex);

                    if (SceneManager.GetActiveScene().buildIndex != data.sceneIndex)
                    {
                        pendingLoadData = data;
                        SceneManager.LoadScene(data.sceneIndex);
                    }
                    else
                    {
                        pendingLoadData = data;
                        StartCoroutine(HandleSceneLoadedCoroutine());
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Hiba a mentés betöltésekor: " + e.Message);
                    pendingLoadData = null;
                    SceneManager.LoadScene(gameStartScene);
                }
            }
        }
        else
        {
            Debug.Log("[GameManager] No save found. Loading default start scene.");
            pendingLoadData = null;
            SceneManager.LoadScene(gameStartScene);
        }
    }

    public void RespawnConfirmed()
    {
        Time.timeScale = 1f;
        RespawnMenuManager.Instance.CloseRespawnMenu();
        EventSystem.current.SetSelectedGameObject(null);

        string path = Application.persistentDataPath + "/" + directoryName + "/" + saveName + ".bin";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream file = File.Open(path, FileMode.Open))
            {
                SaveGameData data = (SaveGameData)formatter.Deserialize(file);

                if (SceneManager.GetActiveScene().buildIndex != data.sceneIndex)
                {
                    pendingLoadData = data;
                    SceneManager.LoadScene(data.sceneIndex);
                }
                else
                {
                    StartCoroutine(ApplyLoadDataCoroutine(data));
                }
            }
        }
        else
        {
            SceneManager.LoadScene(gameStartScene);
        }
    }

    private IEnumerator ApplyLoadDataCoroutine(SaveGameData data)
    {
        yield return null;
        ApplyLoadData(data);
    }

    // ---------------- apply loaded data ----------------
    private void ApplyLoadData(SaveGameData data)
    {
        if (player == null)
        {
            pendingLoadData = data;
            return;
        }

        Debug.Log($"[GameManager] Applying save: pos = ({data.playerPositionX}, {data.playerPositionY})");

        player.transform.position = new Vector3(data.playerPositionX, data.playerPositionY, data.playerPositionZ);

        Rigidbody2D rb2d = player.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.linearVelocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
        }

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.damageMultiplier = data.damageMultiplier;
            PlayerStats.Instance.rangedDamageMultiplier = data.rangedDamageMultiplier;
            PlayerStats.Instance.critChance = data.critChance;
            PlayerStats.Instance.lifeStealAmount = data.lifeStealAmount;

            PlayerStats.Instance.bonusMaxHealth = data.bonusMaxHealth;
            PlayerStats.Instance.bonusMaxPotions = data.bonusMaxPotions;
            PlayerStats.Instance.bonusMaxArrows = data.bonusMaxArrows;

            PlayerStats.Instance.doubleJumpUnlocked = data.doubleJumpUnlocked;
        }



        // --- STATISZTIKÁK VISSZAÁLLÍTÁSA ---
        var dmg = player.GetComponent<Damageable>();
        if (dmg != null)
        {
            dmg.Health = data.health;
            dmg.IsAlive = true;
            dmg.LockVelocity = false;
            dmg.UpdateMaxHealthUI();
        }

        var potion = player.GetComponent<PotionSystem>();
        if (potion != null)
        {
            potion.currentPotions = data.potionAmount;
            if (UIManager.Instance != null) UIManager.Instance.UpdatePotionUI(data.potionAmount);
        }

        var proj = player.GetComponent<ProjectileLauncher>();
        if (proj != null)
        {
            proj.currentArrows = data.arrowAmount;
            if (UIManager.Instance != null) UIManager.Instance.UpdateArrowUI(data.arrowAmount);
        }

        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.LoadExperience(data.currentLevel, data.totalExperience);
        }

        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.LoadSkills(data.skillPoints, data.unlockedSkillIDs);
        }
    }

    // ---------------- death handling ----------------
    private void HandleDeath()
    {
        StartCoroutine(ShowRespawnMenuWithDelay(1f));
    }

    private IEnumerator ShowRespawnMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnMenuManager.Instance.ShowRespawnMenu();
        Time.timeScale = 0f;
    }

    public void LoadMainMenu()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        Time.timeScale = 1f;
        RespawnMenuManager.Instance.CloseRespawnMenu();
        SceneManager.LoadScene("MainMenu");
    }
}