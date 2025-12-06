using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameObject player;
    public GameObject respawnMenu;

    public int gameStartScene = 1;
    public string saveName = "savedGame";
    public string directoryName = "Saves";

    private SaveGameData? pendingLoadData = null;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        ResetSaveData();
        if (respawnMenu != null)
        {
            respawnMenu.SetActive(false);
        }
    }

    public void ResetSaveData()
    {
        string savePath = Application.persistentDataPath + "/" + directoryName;
        string filePath = savePath + "/" + saveName + ".bin";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Mentési fájl törölve a teszteléshez.");
        }
        else
        {
            Debug.Log("Nincs mentési fájl a törléshez.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (scene.buildIndex == 0) return;

        if (player != null)
        {
            Damageable playerDamageable = player.GetComponent<Damageable>();
            if (playerDamageable != null)
            {
                playerDamageable.OnPlayerDied -= HandleDeath;
                playerDamageable.OnPlayerDied += HandleDeath;
            }

            if (pendingLoadData != null)
            {
                ApplyLoadData(pendingLoadData.Value);
                pendingLoadData = null;
            }
        }

        if (respawnMenu != null)
        {
            respawnMenu.SetActive(false);
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        string path = Application.persistentDataPath + "/" + directoryName + "/" + saveName + ".bin";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            SaveGameData data = (SaveGameData)formatter.Deserialize(file);
            file.Close();

            pendingLoadData = data;
            SceneManager.LoadScene(data.sceneIndex);
        }
        else
        {
            pendingLoadData = null;
            SceneManager.LoadScene(gameStartScene);
        }
    }
    public void RespawnConfirmed()
    {
        if (respawnMenu != null)
        {
            respawnMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }

        string path = Application.persistentDataPath + "/" + directoryName + "/" + saveName + ".bin";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            SaveGameData data = (SaveGameData)formatter.Deserialize(file);
            file.Close();

            if (SceneManager.GetActiveScene().buildIndex != data.sceneIndex)
            {
                Time.timeScale = 1f;
                pendingLoadData = data;
                SceneManager.LoadScene(data.sceneIndex);
            }
            else
            {
                ApplyLoadData(data);
            }
        }
        else
        {
            SceneManager.LoadScene(gameStartScene);
        }
    }

    private void ApplyLoadData(SaveGameData data)
    {
        if (player != null)
        {
            player.transform.position = new Vector3(data.playerPositionX, data.playerPositionY, data.playerPositionZ);

            Damageable playerDamageable = player.GetComponent<Damageable>();
            PotionSystem potionSystem = player.GetComponent<PotionSystem>();
            ProjectileLauncher projectileLauncher = player.GetComponent<ProjectileLauncher>();

            if (playerDamageable != null)
            {
                playerDamageable.Health = data.health;
                playerDamageable.IsAlive = true;
                playerDamageable.LockVelocity = false;
            }
            if (potionSystem != null)
            {
                potionSystem.currentPotions = data.potionAmount;
                if (UIManager.Instance != null) UIManager.Instance.UpdatePotionUI(data.potionAmount);
            }
            if (projectileLauncher != null)
            {
                projectileLauncher.currentArrows = data.arrowAmount;
                if (UIManager.Instance != null) UIManager.Instance.UpdateArrowUI(data.arrowAmount);
            }
        }
    }

    private void HandleDeath()
    {
        StartCoroutine(ShowRespawnMenuWithDelay(1f));
    }

    IEnumerator ShowRespawnMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (respawnMenu != null)
        {
            respawnMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void LoadMainMenu()
    {
        player.SetActive(false);
        Time.timeScale = 1f;
        if (PauseMenuManager.isPaused) PauseMenuManager.isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}