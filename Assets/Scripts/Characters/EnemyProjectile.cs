using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public Vector2 knockback = new Vector2(3, 0);

    private Rigidbody2D rb;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(float directionX)
    {
        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * directionX;
        transform.localScale = scale;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(speed * directionX, 0);
        }

        Destroy(gameObject, 0.7f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Player"))
        {
            Damageable playerDamageable = collision.GetComponent<Damageable>();

            if (playerDamageable != null)
            {
                float dir = Mathf.Sign(transform.localScale.x);
                Vector2 finalKnockback = new Vector2(knockback.x * dir, knockback.y);

                bool hitSuccess = playerDamageable.Hit(damage, finalKnockback);

                Destroy(gameObject);

            }
        }
    }
}