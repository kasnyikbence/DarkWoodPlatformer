using UnityEngine;

public class WaterRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private int damageAmount = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            Damageable damageable = collision.GetComponent<Damageable>();


            if (damageable != null)
            {
                damageable.Hit(damageAmount, Vector2.zero);
            }

            collision.transform.position = respawnPoint.position;
            
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }
}
