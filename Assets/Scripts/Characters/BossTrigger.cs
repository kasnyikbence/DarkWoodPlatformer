using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossController boss;
   // public GameObject bossHealthBar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (boss != null)
            {
                boss.ActivateBoss();
            }
            Destroy(gameObject);
        }
    }
}