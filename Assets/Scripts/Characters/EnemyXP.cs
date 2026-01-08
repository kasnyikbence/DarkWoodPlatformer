using UnityEngine;

public class EnemyXP : MonoBehaviour
{
    [Header("XP Beállítások")]
    [Tooltip("Ennyi XP-t ad az ellenfél halálakor")]
    public int xpAmount = 20;

    private Damageable damageable;

    private void Awake()
    {
        damageable = GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        if (damageable != null)
        {
            damageable.OnPlayerDied += GiveXP;
        }
    }

    private void OnDisable()
    {
        if (damageable != null)
        {
            damageable.OnPlayerDied -= GiveXP;
        }
    }

    private void GiveXP()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(xpAmount);
            Debug.Log($"{gameObject.name} legyõzve! Jutalmad: {xpAmount} XP");
        }
        else
        {
            Debug.LogWarning("Nem található ExperienceManager a scene-ben!");
        }
    }
}