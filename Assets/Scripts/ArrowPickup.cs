using UnityEngine;

public class ArrowPickup : MonoBehaviour
{
    public int arrowAmount = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProjectileLauncher launcher = collision.GetComponent<ProjectileLauncher>();
        if (launcher != null)
        {
            launcher.AddArrows(arrowAmount);
            Destroy(gameObject);
        }
    }
}
