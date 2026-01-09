using UnityEngine;

public class RespawnEvents : MonoBehaviour
{
    // Ezt a függvényt kösd be a "Respawn" gombra
    public void OnRespawnClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RespawnConfirmed();
        }
        else
        {
            Debug.LogError("RespawnEvents: Nem található GameManager Instance!");
        }
    }

    public void OnMainMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}