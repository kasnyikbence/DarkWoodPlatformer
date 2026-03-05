using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    private AudioSource audioSource;

    [Range(0f, 1f)]
    public float volume = 1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clipToPlay)
    {
        if (clipToPlay == null || audioSource == null) return;


        audioSource.PlayOneShot(clipToPlay, volume);
    }
}
