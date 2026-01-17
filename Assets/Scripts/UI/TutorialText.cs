using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Beállítások")]
    public GameObject tutorialTextObject;
    public float fadeSpeed = 2f;

    private CanvasGroup canvasGroup;
    void Awake()
    {
        canvasGroup = tutorialTextObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = tutorialTextObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}