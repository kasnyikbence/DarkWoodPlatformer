using UnityEngine;

public class RespawnMenuManager : MonoBehaviour
{
    // Privátra állítjuk, mert a kód keresi meg, nem te húzod be
    private GameObject respawnMenu;

    public static RespawnMenuManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void FindPanelInternal()
    {
        GameObject[] roots = gameObject.scene.GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            Transform[] allChildren = root.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.name == "RespawnCanvas")
                {
                    respawnMenu = child.gameObject;
                    return; 
                }
            }
        }
    }

    public void ShowRespawnMenu()
    {
        if (respawnMenu == null)
        {
            FindPanelInternal();
        }

        if (respawnMenu != null)
        {
            respawnMenu.SetActive(true);
            Debug.Log("[RespawnManager] Menü megjelenítve.");
        }
        else
        {
            Debug.LogError("[RespawnManager] HIBA: Nem találom a 'RespawnCanvas' nevû objektumot a DDOL-ban!");
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