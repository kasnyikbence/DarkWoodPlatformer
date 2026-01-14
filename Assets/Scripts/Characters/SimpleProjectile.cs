using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 15;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Damageable dmg = collision.GetComponent<Damageable>();
            if (dmg != null)
            {
                bool gotHit = dmg.Hit(Mathf.RoundToInt(damage), Vector2.zero);

                if (gotHit)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}