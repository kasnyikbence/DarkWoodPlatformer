using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]

public class PlayerController : MonoBehaviour
{
    [Header("Mozgás")]
    public float runSpeed = 7f;
    public float airWalkSpeed = 7f;
    public float jumpImpulse = 9f;
    public float douleJumpImpulse = 8f;

    [Header("Fizika")]
    public float fallMultiplier = 2.5f;

    [Header("Dash Beállítások")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public GameObject ghostPrefab;
    public float ghostSpawnRate = 0.05f;

    private bool isDashing = false;
    private bool canDash = true;
    private float lastGhostTime;

    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;
    ProjectileLauncher projectileLauncher;
    SpriteRenderer spriteRenderer;

    Rigidbody2D rb;
    Animator animator;

    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove && IsMoving && !touchingDirections.IsOnWall)
            {
                return touchingDirections.IsGrounded ? runSpeed : airWalkSpeed;
            }
            return 0;
        }
    }


    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);

        }
    }

    public bool _isFacingRight = true;


    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;

        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove) && !isDashing;
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        projectileLauncher = GetComponent<ProjectileLauncher>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (touchingDirections.IsGrounded)
        {
            hasDoubleJumped = false;
            canDoubleJump = true;
        }
    }


    private void FixedUpdate()
    {

        if (damageable.LockVelocity || PauseMenuManager.isPaused || DialogueController.isPaused || SkillTreeUI.isOpen)
        {
            return;
        }

        if (isDashing)
        {
            if (Time.time >= lastGhostTime + ghostSpawnRate)
            {
                SpawnGhost();
                lastGhostTime = Time.time;
            }
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused || DialogueController.isPaused || SkillTreeUI.isOpen)
        {
            return;
        }

        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (PauseMenuManager.isPaused)
        {
            return;
        }

        if (moveInput.x > 0 && !IsFacingRight)
        {
            // Face the right
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            // Face the left
            IsFacingRight = false;
        }

    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused || DialogueController.isPaused || SkillTreeUI.isOpen)
        {
            return;
        }
        if (context.started && CanMove)
        {
            if (touchingDirections.IsGrounded)
            {
                animator.SetTrigger(AnimationStrings.jumpTrigger);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);

                canDoubleJump = true;
                hasDoubleJumped = false;
            }
            else
            {
                bool skillUnlocked = PlayerStats.Instance != null && PlayerStats.Instance.doubleJumpUnlocked;

                if (skillUnlocked && canDoubleJump && !hasDoubleJumped)
                {
                    animator.ResetTrigger(AnimationStrings.jumpTrigger);

                    animator.Play("player_jump", -1, 0f);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, douleJumpImpulse);
                    animator.SetFloat(AnimationStrings.yVelocity, jumpImpulse);


                    hasDoubleJumped = true;
                }
            }
        }

    }


    public void OnRoll(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused || DialogueController.isPaused || SkillTreeUI.isOpen)
        {
            return;
        }

        if (context.started && canDash && IsAlive && IsMoving)
        {
            StartCoroutine(DashCoroutine());
        }
    }
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        damageable.IsInvincible = true;

        animator.SetTrigger(AnimationStrings.dashTrigger);

        float dashDirection = IsFacingRight ? 1f : -1f;
        if (moveInput.x != 0) dashDirection = Mathf.Sign(moveInput.x);

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);


        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    private void SpawnGhost()
    {
        if (ghostPrefab != null)
        {
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            ghost.GetComponent<GhostSprite>().Setup(spriteRenderer.sprite, !IsFacingRight);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused || DialogueController.isPaused || SkillTreeUI.isOpen) 
        {
            return;
        }
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }

    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused || DialogueController.isPaused || SkillTreeUI.isOpen || projectileLauncher.currentArrows == 0)
        {
            return;
        }
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
        }

    }

    public void OnHit(int damage, Vector2 knockBack)
    {
        rb.linearVelocity = new Vector2(knockBack.x, rb.linearVelocity.y + knockBack.y);
    }
    public void LockMovement()
    {
        animator.SetBool(AnimationStrings.canMove, false);
    }

    public void UnlockMovement()
    {
        animator.SetBool(AnimationStrings.canMove, true);
    }
}
