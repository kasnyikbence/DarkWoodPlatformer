using System;
using UnityEngine;

[Serializable]
public class BackGroundElement
{
    public SpriteRenderer backgroundSprite;
    [Range(0,1)] public float scrollSpeed;
    [HideInInspector]public Material spriteMaterial;
}

public class ParallaxEffect : MonoBehaviour
{
    private const float SCROLL_MULTIPLIER = 0.01f;
    [SerializeField] private BackGroundElement[] backGroundElements;

    private void Start()
    {
        foreach (BackGroundElement element in backGroundElements)
        {
            element.spriteMaterial = element.backgroundSprite.material;
        }
 }

    private void Update()
    {
        foreach (BackGroundElement element in backGroundElements)
        {
            element.spriteMaterial.mainTextureOffset = 
                new Vector2(transform.position.x * element.scrollSpeed * SCROLL_MULTIPLIER, 0);
        }
    }
}