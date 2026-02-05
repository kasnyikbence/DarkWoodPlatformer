using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    public GameObject loadingScreenContent;
    public Slider progressBar;

    public float minimumLoadTime = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingScreenContent != null)
            loadingScreenContent.SetActive(false);
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncString(sceneName));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        yield return StartCoroutine(LoadingProcess(SceneManager.LoadSceneAsync(sceneIndex)));
    }

    private IEnumerator LoadSceneAsyncString(string sceneName)
    {
        yield return StartCoroutine(LoadingProcess(SceneManager.LoadSceneAsync(sceneName)));
    }

    private IEnumerator LoadingProcess(AsyncOperation operation)
    {
        loadingScreenContent.SetActive(true);
        Time.timeScale = 1f;

        operation.allowSceneActivation = false;

        float timer = 0f;

        if (progressBar != null) progressBar.value = 0f;

        while (!operation.isDone)
        {
            timer += Time.unscaledDeltaTime;

            float actualProgress = Mathf.Clamp01(operation.progress / 0.9f);

            float fakeProgress = Mathf.Clamp01(timer / minimumLoadTime);

            float displayProgress = Mathf.Min(actualProgress, fakeProgress);

            if (progressBar != null) progressBar.value = displayProgress;

            if (operation.progress >= 0.9f && fakeProgress >= 1f)
            {
                if (progressBar != null) progressBar.value = 1f;

                yield return new WaitForSecondsRealtime(0.2f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        loadingScreenContent.SetActive(false);
    }
}