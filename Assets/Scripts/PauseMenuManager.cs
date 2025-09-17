using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsMenu;
    public static bool isPaused = false;
    public GameObject resumeButton;
    public GameObject respawnPanel;

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
        respawnPanel.SetActive(false);
    }
}
