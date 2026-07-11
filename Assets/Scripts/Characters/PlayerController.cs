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
    public bool isAttacking;


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

    private IInteractable currentInteractable;

    Rigidbody2D rb;
    Animator animator;

    private Collider2D[] playerColliders;
    private Collider2D currentOneWayCollider;
    private bool isDropping = false;

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

        playerColliders = GetComponents<Collider2D>();
    }

    private void Update()
    {
        if (touchingDirections.IsGrounded)
        {
            hasDoubleJumped = false;
            canDoubleJump = true;
        }

        if (Keyboard.current != null && (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) && currentOneWayCollider != null && !isDropping)
        {
            StartCoroutine(DisableCollision());
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

    public void OnDash(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused || DialogueController.isPaused || SkillTreeUI.isOpen || isAttacking)
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

        animator.SetBool("isDashing", true);

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

        animator.SetBool("isDashing", false);

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
            if (currentInteractable != null)
            {
                print("Interacting with: " + currentInteractable);
                currentInteractable.Interact();
            }
            else
            {
                isAttacking = true;
                animator.SetTrigger(AnimationStrings.attackTrigger);
            }
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
            isAttacking = true;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null && currentInteractable == interactable)
        {
            currentInteractable = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlatformEffector2D>() != null)
        {
            currentOneWayCollider = collision.collider;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (currentOneWayCollider == null && collision.gameObject.GetComponent<PlatformEffector2D>() != null)
        {
            currentOneWayCollider = collision.collider;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider == currentOneWayCollider)
        {
            currentOneWayCollider = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        if (currentOneWayCollider == null) yield break;

        isDropping = true;

        Collider2D platformToDropThrough = currentOneWayCollider;

        foreach (Collider2D col in playerColliders)
        {
            Physics2D.IgnoreCollision(col, platformToDropThrough, true);
        }

        yield return new WaitForSeconds(0.3f);

        if (platformToDropThrough != null)
        {
            foreach (Collider2D col in playerColliders)
            {
                Physics2D.IgnoreCollision(col, platformToDropThrough, false);
            }
        }

        isDropping = false;
    }

}