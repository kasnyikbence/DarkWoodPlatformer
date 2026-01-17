using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class LootSpawner : MonoBehaviour
{
    public GameObject potionDropPrefab;
    public GameObject arrowDropPrefab;

    [Range(0, 100)] public float dropChance = 50f;
    [Range(0, 100)] public float potionRate = 40f;

    private Damageable damageable;

    void Awake()
    {
        damageable = GetComponent<Damageable>();
    }

    void OnEnable()
    {
        damageable.OnPlayerDied += SpawnLoot;
    }

    void OnDisable()
    {
        damageable.OnPlayerDied -= SpawnLoot;
    }

    void SpawnLoot()
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= dropChance)
        {
            float itemRoll = Random.Range(0f, 100f);
            GameObject itemToSpawn = null;

            if (itemRoll <= potionRate)
            {
                itemToSpawn = potionDropPrefab;
            }
            else
            {
                itemToSpawn = arrowDropPrefab;
            }

            if (itemToSpawn != null)
            {
                Instantiate(itemToSpawn, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
    }
}