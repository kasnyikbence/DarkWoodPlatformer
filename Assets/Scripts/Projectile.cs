using UnityEngine;

public class Projectile : MonoBehaviour
{

    public int damage = 10;
    public Vector2 moveSpeed = new Vector2(10f, 0);
    public Vector2 knockBack = new Vector2(0, 0);

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);

        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        Vector2 deliveredKnockBack = transform.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

        bool gotHit = damageable.Hit(damage, deliveredKnockBack);

        if (gotHit)
        {
            Debug.Log(collision.name + " hit for " + damage);
            Destroy(gameObject);
        }
    }

}
