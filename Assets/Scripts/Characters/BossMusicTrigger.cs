using System.Collections;
using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    public AudioSource[] allAudioSources;
    public AudioSource bossMusic;

    private float fadeDuration = 1.0f;

    void Start()
    {
        if (bossMusic != null && bossMusic.clip != null)
        {
            bossMusic.clip.LoadAudioData();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutAudio(fadeDuration));


            GetComponent<Collider2D>().enabled = false;
        }
    }

    IEnumerator FadeOutAudio(float duration)
    {
        if (bossMusic != null)
        {
            bossMusic.volume = 0;
            bossMusic.Play();
        }

        float[] startVolumes = new float[allAudioSources.Length];
        for (int i = 0; i < allAudioSources.Length; i++)
        {
            if (allAudioSources[i] != null)
            {
                startVolumes[i] = allAudioSources[i].volume;
            }
        }

        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / fadeDuration;

            for (int i = 0; i < allAudioSources.Length; i++)
            {
                if (allAudioSources[i] != null)
                {
                    allAudioSources[i].volume = Mathf.Lerp(startVolumes[i], 0f, percent);
                }
            }

            if (bossMusic != null)
            {
                bossMusic.volume = Mathf.Lerp(0f, 0.0005f, percent);
            }

            yield return null;
        }

        foreach (var source in allAudioSources)
        {
            if (source != null)
            {
                source.volume = 0;
                source.Stop();
            }
        }

        if (bossMusic != null) bossMusic.volume = 0.0005f;
    }
}
