using UnityEngine;

public class PlayOneShotBehavior : StateMachineBehaviour
{
    public AudioClip soundToPlay;
    [Range(0f, 1f)] public float volume = 0.5f;
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false;

    public float playDelay = 0.25f;
    public float timeSinceEntered = 0;
    public bool hasDelayedSoundPlayed = false;

    private AudioSource audioSource;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = animator.gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = animator.gameObject.AddComponent<AudioSource>();

        audioSource.pitch = 1f;

        if (playOnEnter)
        {
            audioSource.PlayOneShot(soundToPlay, volume);
        }

        timeSinceEntered = 0f;
        hasDelayedSoundPlayed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playAfterDelay && !hasDelayedSoundPlayed)
        {
            timeSinceEntered += Time.deltaTime;

            if (timeSinceEntered > playDelay)
            {
                audioSource.pitch = 1f;
                audioSource.PlayOneShot(soundToPlay, volume);
                hasDelayedSoundPlayed = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(soundToPlay, volume);
        }
    }
}
