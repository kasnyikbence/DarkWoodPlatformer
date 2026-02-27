using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public GameObject fadeImage;
    public string sceneToLoad;
    public Vector2 newPlayerPosition;
    private Transform player;
    private float fadeDuration = 1.2f;
    public AudioSource[] allAudioSources;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;

            StartCoroutine(FadeOutAudio(fadeDuration));


            if (fadeImage != null)
            {
                fadeImage.SetActive(true);
                Animator fadeAnim = fadeImage.GetComponent<Animator>();

                if (fadeAnim != null)
                {
                    fadeAnim.Play("FadeToBlack");
                }
            }
        }
    }

    IEnumerator FadeOutAudio(float duration)
    {

        float[] startVolumes = new float[allAudioSources.Length];
        for (int i = 0; i < allAudioSources.Length; i++)
        {
            startVolumes[i] = allAudioSources[i].volume;
        }

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float percent = 1 - (timer / duration);

            for (int i = 0; i < allAudioSources.Length; i++)
            {
                if (allAudioSources[i] != null)
                {
                    allAudioSources[i].volume = startVolumes[i] * percent;
                }
            }
            yield return null;
        }

        foreach (var source in allAudioSources)
        {
            if (source != null) source.volume = 0;
        }
    }

    public void OnFadeComplete()
    {
        if (player != null)
        {
            player.position = newPlayerPosition;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DisablePlayerInput();
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}