using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class EnemyController : MonoBehaviour
{
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float chaseMaxSpeed = 5f;
    public float walkStopRate = 0.05f;

    public float aggroDistance = 5f;
    public float stopChaseDistance = 10f;
    private Transform player;

    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;
    Rigidbody2D rb;

    public enum EnemyPhase
    {
        Patrol,
        Chase,
        Attack
    }

    public EnemyPhase currentPhase = EnemyPhase.Patrol;

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

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool IsMoving
    {
        get
        {
            return animator.GetBool(AnimationStrings.isMoving);
        }
        private set
        {
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
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
        HandleMovePhases();
        HandleMovement();
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
            if (CanMove && currentPhase == EnemyPhase.Patrol)
            {
                rb.linearVelocity = new Vector2(
                    Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed),
                    rb.linearVelocity.y
                );
                IsMoving = true;
            }
            else if (CanMove && currentPhase == EnemyPhase.Chase)
            {
                rb.linearVelocity = new Vector2(
                    Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -chaseMaxSpeed, chaseMaxSpeed),
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

    private void HandleMovePhases()
    {
        if (currentPhase == EnemyPhase.Chase)
        {
            FaceToPlayer();
        }
        else if (currentPhase == EnemyPhase.Patrol)
        {
            if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            {
                FlipDirection();
            }
        }
    }

    private void UpdateEnemyPhase()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (HasTarget)
        {
            currentPhase = EnemyPhase.Attack;
        }
        else if (distance < aggroDistance)
        {
            currentPhase = EnemyPhase.Chase;
            FaceToPlayer();
        }
        else if (distance > stopChaseDistance)
        {
            currentPhase = EnemyPhase.Patrol;
        }
    }

    private void UpdateLockState()
    {
        if (currentPhase == EnemyPhase.Attack)
        {
            damageable.LockVelocity = true;
            animator.SetBool(AnimationStrings.lockVelocity, true);
        }
        else
        {
            damageable.LockVelocity = false;
            animator.SetBool(AnimationStrings.lockVelocity, false);
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Current walkable direction is not set to legal values of right or left");
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

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            if (currentPhase == EnemyPhase.Patrol)
            {
                FlipDirection();
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