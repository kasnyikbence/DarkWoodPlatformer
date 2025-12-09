using UnityEngine;

public class RespawnMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject respawnMenu; // Ezt húzd be az Inspectorban minden pályán!

    public static RespawnMenuManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
        }

        Instance = this;
    }

    public void ShowRespawnMenu()
    {
        if (respawnMenu != null)
        {
            respawnMenu.SetActive(true);
            Debug.Log("[RespawnManager] Menü megjelenítve.");
        }
        else
        {
            Debug.LogError("[RespawnManager] HIBA: Nincs behúzva a respawnMenu panel az Inspectorban!");
        }
    }

    public void CloseRespawnMenu()
    {
        if (respawnMenu != null)
        {
            respawnMenu.SetActive(false);
        }
    }
}