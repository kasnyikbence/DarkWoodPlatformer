using UnityEngine;

public class SceneEntryFader : MonoBehaviour
{
    private Animator animator;

    public string fadeAnimationName = "FadeFromBlack";

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (animator != null && !string.IsNullOrEmpty(fadeAnimationName))
        {
            animator.Play(fadeAnimationName);
        }
        else
        {
            gameObject.SetActive(false);
            Debug.LogError("SceneEntryFader HIBA: Az Animator vagy az animáció neve hiányzik.");
        }
    }

    public void OnFadeInComplete()
    {
        gameObject.SetActive(false);
    }
}