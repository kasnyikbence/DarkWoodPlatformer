using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    [SerializeField][Range(0.0f, 1.0f)] float volume = 1;


    public void playClip()
    {
        if (audioClip != null)
            audioSource.clip = audioClip;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.Play();
    }
}