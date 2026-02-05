using UnityEngine;
using UnityEngine.InputSystem;

public class PotionSystem : MonoBehaviour
{
    [SerializeField] private int baseMaxPotions = 2;
    public int MaxPotions
    {
        get
        {
            int bonus = 0;
            if (PlayerStats.Instance != null)
            {
                bonus = PlayerStats.Instance.bonusMaxPotions;
            }
            return baseMaxPotions + bonus;
        }
    }

    public int currentPotions = 0;
    public int healAmount = 20;

    private Damageable playerDamageable;


    Animator animator;


    private void Awake()
    {
        playerDamageable = GetComponent<Damageable>();
        animator = GetComponent<Animator>();
    }

    public void AddPotion(int amount)
    {
        currentPotions = Mathf.Clamp(currentPotions + amount, 0, MaxPotions);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdatePotionUI(currentPotions);
    }

    public void UsePotion()
    {
        if (currentPotions > 0)
        {
            bool healed = playerDamageable.Heal(healAmount);

            if (healed)
            {
                currentPotions--;
                UIManager.Instance.UpdatePotionUI(currentPotions);

                if (animator != null)
                {
                    animator.SetTrigger(AnimationStrings.potionTrigger);

                }
            }
        }
        else
        {
            Debug.Log("Nincs potion!");
        }
    }

    public void OnUsePotion(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UsePotion();
        }
    }
}
