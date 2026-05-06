using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class RangedEnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float chaseMaxSpeed = 5f;
    public float walkStopRate = 0.05f;

    [Header("Detection")]
    public float aggroDistance = 8f;
    public float stopChaseDistance = 12f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float rangedAttackDelay = 0.5f;

    [Header("Knockback")]
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    // References
    private Transform player;
    private TouchingDirections touchingDirections;
    private Animator animator;
    private Damageable damageable;
    private Rigidbody2D rb;

    public enum EnemyPhase { Patrol, Chase, Attack }
    public EnemyPhase currentPhase = EnemyPhase.Patrol;

    public enum WalkableDirection { Right, Left }
    private WalkableDirection _walkDirections;
    private Vector2 walkDirectionVector = Vector2.right;

    // Attack state
    private bool isAttacking = false;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirections; }
        set
        {
            if (_walkDirections != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                if (value == WalkableDirection.Right) walkDirectionVector = Vector2.right;
                else if (value == WalkableDirection.Left) walkDirectionVector = Vector2.left;
            }
            _walkDirections = value;
        }
    }

    public bool HasTarget { get { return _hasTarget; } private set { _hasTarget = value; animator.SetBool(AnimationStrings.hasTarget, value); } }
    public bool _hasTarget = false;

    public bool CanMove { get { return animator.GetBool(AnimationStrings.canMove); } }

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
        HandleMovePhases();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (currentPhase == EnemyPhase.Attack || isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (!damageable.LockVelocity)
        {
            if (CanMove && currentPhase == EnemyPhase.Patrol)
            {
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.linearVelocity.y);
            }
            else if (CanMove && currentPhase == EnemyPhase.Chase)
            {
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -chaseMaxSpeed, chaseMaxSpeed), rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
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
            damageable.LockVelocity = true;

            if (AttackCooldown <= 0 && !isAttacking)
            {
                StartCoroutine(PerformRangedAttack());
            }
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

    IEnumerator PerformRangedAttack()
    {
        isAttacking = true;
        FaceToPlayer();

        animator.SetBool(AnimationStrings.hasTarget, true);

        yield return new WaitForSeconds(rangedAttackDelay);

        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            EnemyProjectile enemyProj = proj.GetComponent<EnemyProjectile>();

            if (enemyProj != null)
            {
                float dirX = (transform.localScale.x > 0) ? 1f : -1f;

                enemyProj.Launch(dirX);
            }
        }
        yield return new WaitForSeconds(0.5f);

        AttackCooldown = 1f;
        isAttacking = false;
    }

    private void UpdateLockState()
    {
        if (currentPhase == EnemyPhase.Attack)
        {
            damageable.LockVelocity = true;
            animator.SetBool("lockVelocity", true);
        }
        else
        {
            damageable.LockVelocity = false;
            animator.SetBool("lockVelocity", false);
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right) WalkDirection = WalkableDirection.Left;
        else if (WalkDirection == WalkableDirection.Left) WalkDirection = WalkableDirection.Right;
        else Debug.LogError("Current walkable direction is not set to legal values of right or left");
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
        if (touchingDirections.IsGrounded && currentPhase == EnemyPhase.Patrol)
        {
            FlipDirection();
        }
    }

    void FaceToPlayer()
    {
        if (player == null) return;
        if (player.position.x > transform.position.x) WalkDirection = WalkableDirection.Right;
        else WalkDirection = WalkableDirection.Left;
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