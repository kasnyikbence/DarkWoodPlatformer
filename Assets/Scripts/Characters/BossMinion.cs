using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class BossMinion : MonoBehaviour
{
    public float walkAcceleration = 30f;
    public float maxSpeed = 5f;
    public float walkStopRate = 0.05f;

    public DetectionZone attackZone;

    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    private Transform player;
    private TouchingDirections touchingDirections;
    private Animator animator;
    private Damageable damageable;
    private Rigidbody2D rb;

    public enum EnemyPhase { Chase, Attack }
    public EnemyPhase currentPhase = EnemyPhase.Chase;

    public enum WalkableDirection { Right, Left }
    private WalkableDirection _walkDirections;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirections; }
        set
        {
                if (_walkDirections != value)
                {
                    gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                    if (value == WalkableDirection.Right)
                    {
                        walkDirectionVector = Vector2.right;
                    }
                    else if (value == WalkableDirection.Left)
                    {
                        walkDirectionVector = Vector2.left;
                    }
                }
                _walkDirections = value;
        }
    }

    public bool HasTarget { get { return _hasTarget; } private set { _hasTarget = value; animator.SetBool(AnimationStrings.hasTarget, value); } }
    private bool _hasTarget = false;

    public bool CanMove { get { return animator.GetBool(AnimationStrings.canMove); } }

    [SerializeField] private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    public float AttackCooldown
    {
        get { return animator.GetFloat(AnimationStrings.attackCooldown); }
        private set { animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0)); }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        HasTarget = attackZone.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

        UpdateEnemyPhase();
    }

    void FixedUpdate()
    {
        if (isKnockedBack) return;

        UpdateLockState();
        HandleOrientation();
        HandleMovement();
    }

    private void UpdateEnemyPhase()
    {
        if (HasTarget)
        {
            currentPhase = EnemyPhase.Attack;
        }
        else
        {
            currentPhase = EnemyPhase.Chase;
        }
    }

    private void UpdateLockState()
    {
        if (currentPhase == EnemyPhase.Attack)
        {
            damageable.LockVelocity = true;
        }
        else
        {
            damageable.LockVelocity = false;
        }
    }

    private void HandleOrientation()
    {
        if (currentPhase == EnemyPhase.Chase)
        {
            FaceToPlayer();
        }
    }

    private void HandleMovement()
    {
        if (currentPhase == EnemyPhase.Attack)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            IsMoving = false;
            return;
        }


        if (!damageable.LockVelocity)
        {
            if (CanMove && currentPhase == EnemyPhase.Chase)
            {
                rb.linearVelocity = new Vector2(
                    Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed),
                    rb.linearVelocity.y
                );
                IsMoving = true;
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
                IsMoving = false;
            }
        }
    }

    void FaceToPlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            WalkDirection = WalkableDirection.Left;
        }
    }

    public void OnHit(int damage, Vector2 knockBack)
    {
        rb.linearVelocity = new Vector2(knockBack.x, rb.linearVelocity.y + knockBack.y);
        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    private void FindPlayer()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        else
        {
            var allDamageables = Resources.FindObjectsOfTypeAll<Damageable>();
            foreach (var d in allDamageables)
            {
                if (d.gameObject.scene.IsValid() && d.gameObject.CompareTag("Player"))
                {
                    player = d.gameObject.transform;
                    break;
                }
            }
        }
    }
}