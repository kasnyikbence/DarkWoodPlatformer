using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private int playerDamage = 50;
    [SerializeField] private float respawnDelay = 0.5f;
    [SerializeField] private float knockbackForce = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Damageable damageable = collision.GetComponent<Damageable>();
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (damageable != null)
            {
                damageable.Hit(playerDamage, Vector2.zero);
            }

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(Vector2.up * knockbackForce, ForceMode2D.Impulse);
            }

            if (damageable.IsAlive)
            {
                StartCoroutine(RespawnAfterDelay(collision.transform, rb));

            }
        }

        if (collision.CompareTag("Enemy"))
        {
            Damageable damageable = collision.GetComponent<Damageable>();
            EnemyXP xpScript = collision.GetComponent<EnemyXP>();
            LootSpawner lootSpawner = collision.GetComponent<LootSpawner>();

            if (xpScript != null && lootSpawner != null)
            {
                lootSpawner.enabled = false;
                xpScript.enabled = false;
            }

            damageable.Hit(damageable.Health, Vector2.zero);

        }
    }

    private IEnumerator RespawnAfterDelay(Transform playerTransform, Rigidbody2D rb)
    {
        yield return new WaitForSeconds(respawnDelay);

        playerTransform.position = respawnPoint.position;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
