using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{

    public Transform launchPoint;
    public GameObject projectilePrefab;

    [Header("Arrow Count")]
    [SerializeField] private int baseMaxArrows = 5;
    public int MaxArrows
    {
        get
        {
            int bonus = 0;
            if (PlayerStats.Instance != null)
            {
                bonus = PlayerStats.Instance.bonusMaxArrows;
            }
            return baseMaxArrows + bonus;
        }
    }

    public int currentArrows;

    void Awake()
    {
        currentArrows = 0;
    }

    void Start()
    {
        if (currentArrows == 0)
        {
            currentArrows = MaxArrows;
        }

        if (currentArrows > MaxArrows) currentArrows = MaxArrows;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateArrowUI(currentArrows);
        }
    }


    public void FireProjectile()
    {

        if (currentArrows <= 0)
        {
            Debug.Log("Nincs több nyíl!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Vector3 origScale = projectile.transform.localScale;
        projectile.transform.localScale = new Vector3(
            origScale.x * transform.localScale.x > 0 ? 1 : -1,
            origScale.y,
            origScale.z
            );

        currentArrows--;
        UIManager.Instance.UpdateArrowUI(currentArrows);

    }
    public void AddArrows(int amount)
    {
        currentArrows = Mathf.Clamp(currentArrows + amount, 0, MaxArrows);
        UIManager.Instance.UpdateArrowUI(currentArrows);
    }

}
