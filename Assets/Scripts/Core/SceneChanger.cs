using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public GameObject fadeImage;
    public string sceneToLoad;
    public Vector2 newPlayerPosition;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;

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

    public void OnFadeComplete()
    {
        if (player != null)
        {
            player.position = newPlayerPosition;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}