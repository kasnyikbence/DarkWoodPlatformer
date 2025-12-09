using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEngine.UIElements.UxmlAttributeDescription;

// NOTE: BinaryFormatter is obsolete/unsafe for production; OK for prototípus.
// Replace with a safer serializer later (JSON, protobuf, etc.).
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

        Debug.Log("[GameManager] Awake - singleton ready");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ---------------- public helpers ----------------
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
    private void Start()
    {
        //DeleteSaveData();
    }

    // ---------------- scene load handling ----------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene loaded: {scene.name} (buildIndex {scene.buildIndex}).");

        // If main menu (assume buildIndex 0 = MainMenu), deactivate player so it won't fall
        if (scene.buildIndex == 0)
        {
            // try to find an existing persistent player and hide it in menu
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.SetActive(false);
                Debug.Log("[GameManager] Player deactivated for MainMenu.");
            }
            // nothing more to do on main menu
            return;
        }

        // For gameplay scenes, we need to ensure player exists before applying pending save data.
        // Use coroutine to wait for the Player object to appear (it might be created by persistent root or instantiated).
        StartCoroutine(HandleSceneLoadedCoroutine());
    }

    private IEnumerator HandleSceneLoadedCoroutine()
    {
        yield return null;

        // Wait until Player exists, no timeout
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        // Activate if disabled
        if (!player.activeInHierarchy)
            player.SetActive(true);

        // Hook death logic
        var dmg = player.GetComponent<Damageable>();
        if (dmg != null)
        {
            dmg.OnPlayerDied -= HandleDeath;
            dmg.OnPlayerDied += HandleDeath;
        }

        if (pendingLoadData.HasValue)
        {
            SaveGameData data = pendingLoadData.Value;
            pendingLoadData = null;

            // ensure full initialization before applying save data
            yield return null;

            ApplyLoadData(data);
        }
    }

    // ---------------- start / load game ----------------
    public void StartGame()
    {
        Time.timeScale = 1f;

        string path = Application.persistentDataPath + "/" + directoryName + "/" + saveName + ".bin";

        if (File.Exists(path))
        {
            // load file
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream file = File.Open(path, FileMode.Open))
            {
                SaveGameData data = (SaveGameData)formatter.Deserialize(file);
                Debug.Log("[GameManager] Save found. Loading scene index: " + data.sceneIndex);

                // if different scene, set pending and load that scene (ApplyLoadData will run when player is available)
                if (SceneManager.GetActiveScene().buildIndex != data.sceneIndex)
                {
                    pendingLoadData = data;
                    SceneManager.LoadScene(data.sceneIndex);
                }
                else
                {
                    // same scene - apply immediately (but player must exist; coroutine will ensure it later)
                    pendingLoadData = data;
                    // try apply if player already present (safe-guard)
                    if (player != null)
                    {
                        ApplyLoadData(data);
                        pendingLoadData = null;
                    }
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

    // ---------------- respawn flow ----------------
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
                    // load the saved scene first, then apply data
                    pendingLoadData = data;
                    SceneManager.LoadScene(data.sceneIndex);
                }
                else
                {
                    // same scene -> apply directly
                    ApplyLoadData(data);
                }
            }
        }
        else
        {
            SceneManager.LoadScene(gameStartScene);
        }
    }

    // ---------------- apply loaded data ----------------
    private void ApplyLoadData(SaveGameData data)
    {
        if (player == null)
        {
            pendingLoadData = data;
            return;
        }

        Debug.Log($"[GameManager] Applying save: pos=({data.playerPositionX},{data.playerPositionY},{data.playerPositionZ}) HP={data.health} pot={data.potionAmount} arrows={data.arrowAmount}");


        // set position
        player.transform.position = new Vector3(data.playerPositionX, data.playerPositionY, data.playerPositionZ);

        // restore stats
        var dmg = player.GetComponent<Damageable>();
        if (dmg != null)
        {
            dmg.Health = data.health;
            dmg.IsAlive = true;
            dmg.LockVelocity = false;
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
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
