using Unity.Mathematics;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    public int healthRestore = 20;
    public float amplitude = 0.25f;
    public float speed = 1.5f;  
    public Vector3 startPos;

    AudioSource pickupSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        pickupSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        startPos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable)
        {
            bool wasHealed = damageable.Heal(healthRestore);

            if (wasHealed)
            {
                if (pickupSource)
                {
                    AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
                }            
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
