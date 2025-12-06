using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;

    public GameObject pauseMenuUI;

    private InputAction pauseAction;
    public static bool isPaused = false;

    void Awake()
    {
        pauseMenuUI.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
        }
        else
        {
            FindAndSubscribeToInput();
        }
    }

    void FindAndSubscribeToInput()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerInput input = player.GetComponent<PlayerInput>();
            if (input != null)
            {
                pauseAction = input.actions.FindAction("Pause");
                if (pauseAction != null)
                {
                    pauseAction.performed -= TogglePause;
                    pauseAction.performed += TogglePause;
                    pauseAction.Enable();
                }
            }
        }
    }
    private void TogglePause(InputAction.CallbackContext context)
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    public void LoadMainMenu()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}