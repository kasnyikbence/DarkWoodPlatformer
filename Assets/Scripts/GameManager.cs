using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;
    public GameObject respawnMenu;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Damageable playerDamageable = player.GetComponent<Damageable>();

        if (playerDamageable != null)
        {
            playerDamageable.OnPlayerDied += HandleDeath;
        }

        if (respawnMenu != null)
        {
            respawnMenu.SetActive(false);
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
        }

        // Ellenőrizzük, van-e mentett checkpoint
        if (PlayerPrefs.HasKey("CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");

            Vector3 savedPosition = new Vector3(x, y, z);

            player.transform.position = savedPosition;

            Damageable playerDamageable = player.GetComponent<Damageable>();
            if (playerDamageable != null)
            {
                playerDamageable.Health = playerDamageable.MaxHealth;
                playerDamageable.IsAlive = true;
                playerDamageable.LockVelocity = false;
            }
        }
        else
        {
            Debug.Log("Nincs mentett checkpoint, nem tudunk újjáéledni.");
        }
    }
}