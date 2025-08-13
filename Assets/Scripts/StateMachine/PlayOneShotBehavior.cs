using UnityEngine;

public class PlayOneShotBehavior : StateMachineBehaviour
{
    public AudioClip soundToPlay;
    public float volume = 1f;
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false;

    public float playDelay = 0.25f;
    public float timeSinceEntered = 0;
    public bool hasDelayedSoundPlayed = false;

    private AudioSource audioSource;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Lekéri az AudioSource komponenst, vagy hozzáadja, ha még nincs
        audioSource = animator.gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = animator.gameObject.AddComponent<AudioSource>();
        }

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
            audioSource.PlayOneShot(soundToPlay, volume);
        }
    }
}
