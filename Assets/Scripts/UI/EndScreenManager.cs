using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    [Header("UI Referenciák")]
    public GameObject victoryPanel; 
    public CanvasGroup canvasGroup;

    [Header("Beállítások")]
    public float fadeDuration = 2f; 

    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            Time.timeScale = 0f;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                StartCoroutine(FadeInRoutine());
            }
        }
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {

            timer += Time.unscaledDeltaTime;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            }

            yield return null;
        }

        if (canvasGroup != null) canvasGroup.alpha = 1;
    }

    public void LoadMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}