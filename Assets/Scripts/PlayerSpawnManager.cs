using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
    private Transform player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player == null) return;

        SpawnPoint[] spawnPoints =
            Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.spawnID == "CaveSpawnPoint")
            {
                player.position = sp.transform.position;
                return;
            }
        }
    }
}
