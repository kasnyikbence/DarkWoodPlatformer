using Unity.Mathematics;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    public int healthRestore = 20;
    public float amplitude = 0.25f;
    public float speed = 1.5f;  
    public Vector3 startPos;
    public int amount = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        startPos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PotionSystem potionSystem = collision.GetComponent<PotionSystem>();

        if (potionSystem)
        {
            if (potionSystem.currentPotions != potionSystem.MaxPotions)
            {
                potionSystem.AddPotion(amount);
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        float yOffSet = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPos.x, startPos.y + yOffSet, startPos.z);
    }

}
