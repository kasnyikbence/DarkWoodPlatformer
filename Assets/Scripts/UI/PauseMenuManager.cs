using System.Collections;
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
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        if (Instance == null)
        {
            Instance = this;
            // Ha ez egy UI objektumon van, ami nem DDOL, akkor ez a sor nem kell.
            // De ha DDOL-ban van, akkor maradjon.
            DontDestroyOnLoad(gameObject);
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
            StopAllCoroutines();
            StartCoroutine(SubscribeToInputRoutine());
        }
    }

    IEnumerator SubscribeToInputRoutine()
    {
        GameObject player = null;

        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        yield return null;

        PlayerInput input = player.GetComponent<PlayerInput>();
        if (input != null)
        {
            if (pauseAction != null)
            {
                pauseAction.performed -= TogglePause;
                pauseAction.Disable();
            }

            pauseAction = input.actions.FindAction("Pause");
            if (pauseAction != null)
            {
                pauseAction.performed += TogglePause;
                pauseAction.Enable();
            }
            else
            {
                Debug.LogError("[PauseMenuManager] Nem található a 'Pause' action a PlayerInput-ban!");
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
            UIManager.Instance.HideInteractHint();
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