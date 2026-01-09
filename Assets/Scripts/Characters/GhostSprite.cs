using UnityEngine;

public class GhostSprite : MonoBehaviour
{
    public float activeTime = 0.1f;
    public float fadeSpeed = 2f;

    private SpriteRenderer mySprite;
    private SpriteRenderer playerSprite;
    private float timeActive;
    private Color color;

    private void Awake()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    public void Setup(Sprite sprite, bool flipX)
    {
        mySprite.sprite = sprite;
        mySprite.flipX = flipX;
        color = mySprite.color;
        timeActive = activeTime;
    }

    private void Update()
    {
        if (timeActive > 0)
        {
            timeActive -= Time.deltaTime;
        }
        else
        {
            color.a -= fadeSpeed * Time.deltaTime;
            mySprite.color = color;

            if (color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}