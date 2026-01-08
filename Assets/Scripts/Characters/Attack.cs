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

            float finalDamage = attackDamage;
            bool isCritical = false;

            bool isPlayerAttack = transform.root.CompareTag("Player");


            if (isPlayerAttack && PlayerStats.Instance != null)
            {
                //Warrior
                finalDamage *= PlayerStats.Instance.damageMultiplier;

                if (Random.value < PlayerStats.Instance.critChance)
                {
                    //Crit Strike
                    finalDamage *= PlayerStats.Instance.critMultiplier;
                    isCritical = true;
                }
            }

            int damageToSend = Mathf.RoundToInt(finalDamage);

            bool gotHit = damageable.Hit(damageToSend, deliveredKnockBack);

            if (gotHit)
            {
                Debug.Log($"{collision.name} hit for {damageToSend} {(isCritical ? "(CRIT!)" : "")}");

                //Life Steal
                if (isPlayerAttack && PlayerStats.Instance != null && PlayerStats.Instance.lifeStealAmount > 0)
                {
                    Damageable playerHealth = transform.root.GetComponent<Damageable>();
                    if (playerHealth != null)
                    {
                        playerHealth.Heal(Mathf.RoundToInt(PlayerStats.Instance.lifeStealAmount));
                    }
                }
            }

        }
    }
}
