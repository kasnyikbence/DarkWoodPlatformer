using UnityEngine;
using System;

public class EnemyStateManager : MonoBehaviour
{
    [Header("ID")]
    public string enemyID;

    private Damageable damageable;

    private void Awake()
    {
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsEnemyDead(enemyID))
            {
                gameObject.SetActive(false);
                return;
            }
        }

        if (damageable != null)
        {
            damageable.OnPlayerDied += HandleDeath;
        }
    }

    private void OnDestroy()
    {
        if (damageable != null) damageable.OnPlayerDied -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterDeadEnemy(enemyID);
        }
    }

    [ContextMenu("Generate Unique ID")]
    private void GenerateID()
    {
        enemyID = Guid.NewGuid().ToString();
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(enemyID) && gameObject.scene.IsValid())
        {
            enemyID = Guid.NewGuid().ToString();
        }
    }
}