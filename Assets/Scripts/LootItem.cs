using UnityEngine;

public class LootItem : MonoBehaviour
{
    public enum LootType
    {
        Potion,
        Arrow
    }
    public LootType lootType;
    public int amount = 1;

    [Header("Beállítások")]
    public LayerMask groundLayer;
    public float groundCheckDist = 0.5f;

    [Header("Animáció")]
    public float hoverSpeed = 3f;
    public float hoverAmplitude = 0.2f;

    private Rigidbody2D rb;
    private bool hasLanded = false;
    private Vector3 landPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (rb != null)
        {
            float randomX = Random.Range(-0.5f, 0.5f);
            rb.linearVelocity = new Vector2(randomX, 4f);
        }

        Destroy(gameObject, 60f);
    }

    void FixedUpdate()
    {
        if (!hasLanded && rb.linearVelocity.y <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, groundLayer);

            if (hit.collider != null)
            {
                LandOnGround(hit.point);
            }
        }
    }

    void Update()
    {
        if (hasLanded)
        {
            float newY = landPosition.y + 0.5f + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
            transform.position = new Vector3(landPosition.x, newY, transform.position.z);
        }
    }

    private void LandOnGround(Vector2 groundPoint)
    {
        hasLanded = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        landPosition = new Vector3(transform.position.x, groundPoint.y, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TryPickup(collision.gameObject);
        }
    }

    private void TryPickup(GameObject collector)
    {
        bool pickedUp = false;

        if (lootType == LootType.Potion)
        {
            PotionSystem potionSystem = collector.GetComponent<PotionSystem>();
            if (potionSystem != null)
            {
                int space = potionSystem.MaxPotions - potionSystem.currentPotions;
                if (space > 0)
                {
                    int add = Mathf.Min(amount, space);
                    potionSystem.AddPotion(add);
                    if (UIManager.Instance) UIManager.Instance.ShowPickupMessage("+" + add + " Potion");
                    pickedUp = true;
                }
                else
                {
                    if (UIManager.Instance) UIManager.Instance.ShowPickupMessage("Potions full!");
                }
            }
        }
        else if (lootType == LootType.Arrow)
        {
            ProjectileLauncher launcher = collector.GetComponent<ProjectileLauncher>();
            if (launcher != null)
            {
                int space = launcher.MaxArrows - launcher.currentArrows;
                if (space > 0)
                {
                    int add = Mathf.Min(amount, space);
                    launcher.AddArrows(add);
                    if (UIManager.Instance) UIManager.Instance.ShowPickupMessage("+" + add + " Arrow");
                    pickedUp = true;
                }
                else
                {
                    if (UIManager.Instance) UIManager.Instance.ShowPickupMessage("Arrows full!");
                }
            }
        }

        if (pickedUp)
        {
            Destroy(gameObject);
        }
    }
}