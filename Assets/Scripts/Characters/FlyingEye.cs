using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Damageable))]
public class FlyingEye : MonoBehaviour
{
    public float flightSpeed = 3f;
    public float chaseSpeed = 5f;
    public float waypointReachedDistance = 0.1f;

    public float aggroDistance = 6f;
    public float stopChaseDistance = 12f;
    public DetectionZone biteDetectionZone;

    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;

    public List<Transform> waypoints;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;
    Collider2D col;
    Transform player;

    Transform nextWaypoint;
    int waypointNum = 0;
    bool isDeadOnGround = false;


    public enum FlyingEyePhase { Patrol, Chase, Attack }
    public FlyingEyePhase currentState = FlyingEyePhase.Patrol;

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (waypoints.Count > 0)
        {
            nextWaypoint = waypoints[waypointNum];
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (!damageable.IsAlive) return;

        HasTarget = biteDetectionZone.detectedColliders.Count > 0;

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (HasTarget)
        {
            currentState = FlyingEyePhase.Attack;
        }
        else if (distanceToPlayer < aggroDistance)
        {
            currentState = FlyingEyePhase.Chase;
        }
        else if (distanceToPlayer > stopChaseDistance)
        {
            currentState = FlyingEyePhase.Patrol;
        }
    }

    void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                rb.gravityScale = 0f;

                if (currentState == FlyingEyePhase.Patrol)
                {
                    PatrolFlight();
                }
                else if (currentState == FlyingEyePhase.Chase)
                {
                    ChaseFlight();
                }
                else if (currentState == FlyingEyePhase.Attack)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            if (isDeadOnGround)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }


            rb.gravityScale = 2f;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (col != null && col.isTrigger)
            {
                col.isTrigger = false;
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

            if (hit.collider != null)
            {
                isDeadOnGround = true;

                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;

                float spriteHalfHeight = 0.5f;
                transform.position = new Vector3(transform.position.x, hit.point.y + spriteHalfHeight, transform.position.z);

                if (col != null) col.isTrigger = true;
            }
        }
    }

    private void PatrolFlight()
    {
        if (waypoints.Count == 0) return;

        Vector2 direction = (nextWaypoint.position - transform.position).normalized;
        rb.linearVelocity = direction * flightSpeed;

        UpdateDirection(direction.x);

        if (Vector2.Distance(nextWaypoint.position, transform.position) <= waypointReachedDistance)
        {
            waypointNum++;
            if (waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }
            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void ChaseFlight()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;

        UpdateDirection(direction.x);
    }

    public void UpdateDirection(float velocityX)
    {
        Vector3 locScale = transform.localScale;

        if (velocityX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(locScale.x), locScale.y, locScale.z);
        }
        else if (velocityX < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(locScale.x), locScale.y, locScale.z);
        }
    }
}