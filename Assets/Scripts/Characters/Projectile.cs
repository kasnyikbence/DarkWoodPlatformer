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

    
    void Start()
    {
        rb.linearVelocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockBack = transform.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

            float finalDamage = damage;
            bool isCritical = false;

            if (PlayerStats.Instance != null)
            {
                finalDamage *= PlayerStats.Instance.rangedDamageMultiplier;
            }

            if (Random.value < PlayerStats.Instance.critChance)
            {
                finalDamage *= PlayerStats.Instance.critMultiplier;
                isCritical = true;
            }

            bool gotHit = damageable.Hit(Mathf.RoundToInt(finalDamage), deliveredKnockBack);

            if (gotHit)
            {
                Debug.Log($"{collision.name} hit for {finalDamage} {(isCritical ? "(CRIT!)" : "")}");
                Destroy(gameObject);
            }
        }
    }

}
