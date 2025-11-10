using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameObject player;
    public GameObject respawnMenu;
    public int gameStartScene;
    public string saveName = "savedGame";
    public string directoryName = "Saves";
    [SerializeField] private Transform initialSpawnPoint;

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
    void Start()
    {
        ResetSaveData();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Damageable playerDamageable = player.GetComponent<Damageable>();

            if (playerDamageable != null)
            {
                playerDamageable.OnPlayerDied += HandleDeath;
            }
        }
        LoadGame();

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

    private void HandleDeath()
    {
        StartCoroutine(ShowRespawnMenuWithDelay(2f));
    }

    IEnumerator ShowRespawnMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (respawnMenu != null)
        {
            respawnMenu.SetActive(true);
        }
    }

    public void RespawnConfirmed()
    {
        if (respawnMenu != null)
        {
            respawnMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }

        string savePath = Application.persistentDataPath + "/" + directoryName;
        string filePath = savePath + "/" + saveName + ".bin";

        Vector3 respawnPosition = Vector3.zero;
        bool loadedFromFile = false;

        if (initialSpawnPoint != null)
        {
            respawnPosition = initialSpawnPoint.position;
        }
        else
        {
            Debug.LogError("Az Initial Spawn Point nincs beállítva a GameManager-ben! Visszaesés a (0,0,0) pozícióra.");
        }

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream saveFile = File.Open(filePath, FileMode.Open);

            SaveGameData loadData = (SaveGameData)formatter.Deserialize(saveFile);
            saveFile.Close();

            respawnPosition = new Vector3(loadData.playerPositionX, loadData.playerPositionY, loadData.playerPositionZ);
            loadedFromFile = true;

            Damageable playerDamageable = player.GetComponent<Damageable>();
            PotionSystem potionSystem = player.GetComponent<PotionSystem>();
            ProjectileLauncher projectileLauncher = player.GetComponent<ProjectileLauncher>();

            if (playerDamageable != null)
            {
                playerDamageable.Health = loadData.health;
                playerDamageable.IsAlive = true;
                playerDamageable.LockVelocity = false;
            }

            if (potionSystem != null)
            {
                potionSystem.currentPotions = loadData.potionAmount;
                UIManager.Instance.UpdatePotionUI(loadData.potionAmount);
            }
            if (projectileLauncher != null)
            {
                projectileLauncher.AddArrows(loadData.arrowAmount - projectileLauncher.maxArrows);
                UIManager.Instance.UpdateArrowUI(loadData.arrowAmount);
            }

            Debug.Log("Újraéledés mentett Checkpointról.");
        }
        else
        {
            Debug.Log("Nincs mentési fájl. Újraéledés az alapértelmezett kezdőponton.");
        }

        if (player != null)
        {
            player.transform.position = respawnPosition;

            if (loadedFromFile == false)
            {
                Damageable playerDamageable = player.GetComponent<Damageable>();
                if (playerDamageable != null)
                {
                    playerDamageable.Health = playerDamageable.MaxHealth;
                    playerDamageable.IsAlive = true;
                    playerDamageable.LockVelocity = false;
                }
            }
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        if (PauseMenuManager.isPaused)
        {
            PauseMenuManager.isPaused = false;
        }
        SceneManager.LoadScene(gameStartScene);
    }

    private void LoadGame()
    {
        string savePath = Application.persistentDataPath + "/" + directoryName;
        string filePath = savePath + "/" + saveName + ".bin";

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream saveFile = File.Open(filePath, FileMode.Open);
            SaveGameData loadData = (SaveGameData)formatter.Deserialize(saveFile);
            saveFile.Close();

            Debug.Log("Játék betöltve a legutolsó mentésből.");

            Vector3 savedPosition = new Vector3(loadData.playerPositionX, loadData.playerPositionY, loadData.playerPositionZ);

            if (player != null)
            {
                player.transform.position = savedPosition;
                Damageable playerDamageable = player.GetComponent<Damageable>();
                PotionSystem potionSystem = player.GetComponent<PotionSystem>();
                ProjectileLauncher projectileLauncher = player.GetComponent<ProjectileLauncher>();

                if (playerDamageable != null)
                {
                    playerDamageable.Health = loadData.health;
                }

                if (potionSystem != null)
                {
                    potionSystem.currentPotions = loadData.potionAmount;
                    UIManager.Instance.UpdatePotionUI(loadData.potionAmount);
                }

                if (projectileLauncher != null)
                {
                    projectileLauncher.currentArrows = loadData.arrowAmount;
                    UIManager.Instance.UpdateArrowUI(loadData.arrowAmount);
                }
            }
        }
        else
        {
            Debug.Log("Nincs mentési fájl, a játékos az alapértelmezett pozíción indul.");
        }
    }


    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        if (PauseMenuManager.isPaused)
        {
            PauseMenuManager.isPaused = false;
        }
        PauseMenuManager.isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}