using UnityEngine;

public class PlayLoopingSoundBehavior : StateMachineBehaviour
{
    public AudioClip soundToPlay;
    [Range(0f, 1f)] public float volume = 0.5f;

    public float loopInterval = 0.3f;

    public float pitchVariance = 0.1f;

    private float timer = 0f;
    private AudioSource audioSource;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = animator.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = animator.gameObject.AddComponent<AudioSource>();
        }

        PlaySound();
        timer = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer >= loopInterval)
        {
            PlaySound();
            timer = 0f;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource.pitch = 1f;
    }

    private void PlaySound()
    {
        if (soundToPlay != null && audioSource != null)
        {
            audioSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);

            audioSource.PlayOneShot(soundToPlay, volume);
        }
    }
}