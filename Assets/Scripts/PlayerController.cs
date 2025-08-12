using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography.X509Certificates;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]

public class PlayerController : MonoBehaviour
{

    public float runSpeed = 7f;
    public float airWalkSpeed = 7f;
    public float jumpImpulse = 7f;
    public float slideImpulse = 5f;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;

    Rigidbody2D rb;
    Animator animator;

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
            return animator.GetBool(AnimationStrings.canMove);
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
    }

    private void FixedUpdate()
    {

        if (damageable.LockVelocity || PauseMenuManager.isPaused)
        {
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused)
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
        if (PauseMenuManager.isPaused)
        {
            return;
        }
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }

    }


    public void OnRoll(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused)
        {
            return;
        }
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.slideTrigger);
            rb.linearVelocity = new Vector2(slideImpulse * (IsFacingRight ? 1 : -1), rb.linearVelocity.y);
            damageable.IsInvincible = true;

        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.isPaused)
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
        if (PauseMenuManager.isPaused)
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
}
