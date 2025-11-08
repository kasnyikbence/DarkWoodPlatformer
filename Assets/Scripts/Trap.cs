using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private int damageAmount = 50;
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
                damageable.Hit(damageAmount, Vector2.zero);
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
