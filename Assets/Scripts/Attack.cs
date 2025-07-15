using UnityEngine;

public class Attack : MonoBehaviour
{

    Collider2D attackCollider;
    public int attackDamage = 10;

    public Vector2 knockBack = Vector2.zero;
    void Awake()
    {
        attackCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockBack = transform.parent.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockBack);
            
            if (gotHit)
                Debug.Log(collision.name + " hit for " + attackDamage);
            
        }
    }
}
