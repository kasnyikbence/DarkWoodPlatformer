using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements.Experimental;

public class Damageable : MonoBehaviour
{

    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<int, int> healthChanged;

    Animator animator;
    public float timeSinceHit = 0;
    public float invincibilityTimer = 0.25f;
    public event Action OnPlayerDied;

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool _isInvincible = false;

    public bool IsInvincible
    {
        get
        {
            return _isInvincible;
        }
        set
        {
            _isInvincible = value;
        }
    }

    [SerializeField] private int _baseMaxHealth = 100;
    public int MaxHealth
    {
        get
        {
            int bonus = 0;
            if (gameObject.CompareTag("Player") && PlayerStats.Instance != null)
            {
                bonus = PlayerStats.Instance.bonusMaxHealth;
            }
            return _baseMaxHealth + bonus;
        }
    }

    [SerializeField]
    private int _health = 100;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (_health > MaxHealth) _health = MaxHealth;

            healthChanged?.Invoke(_health, MaxHealth);

            if (_health <= 0)
            {
                IsAlive = false;
                OnPlayerDied?.Invoke();
            }
        }
    }

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log(gameObject.name + " IsAlive set to " + value);

        }
    }
    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }

    }

    private void Start()
    {
        healthChanged?.Invoke(Health, MaxHealth);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_isInvincible)
        {
            if (timeSinceHit > invincibilityTimer)
            {
                _isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector2 knockBack)
    {
        if (IsAlive && !_isInvincible)
        {
            Health -= damage;
            _isInvincible = true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockBack);
            CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

           // HitStop.Instance.StopTime(0.5f);
            GetComponent<Unity.Cinemachine.CinemachineImpulseSource>().GenerateImpulse();

            return true;
        }
        return false;
    }

    public bool Heal(int healthRestore)
    {
        if (IsAlive && Health < MaxHealth)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;

            CharacterEvents.characterHealed(gameObject, actualHeal);
            return true;
        }
        return false;
    }

    public void UpdateMaxHealthUI()
    {
        healthChanged?.Invoke(Health, MaxHealth);
    }
}
