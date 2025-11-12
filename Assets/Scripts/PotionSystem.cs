using UnityEngine;
using UnityEngine.InputSystem;

public class PotionSystem : MonoBehaviour
{
    public int maxPotions = 2;
    public int currentPotions = 0;
    public int healAmount = 20;

    private Damageable playerDamageable;

    [Header("Audio")]
    public AudioClip pickupClip;
    private AudioSource audioSource;
    Animator animator;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerDamageable = GetComponent<Damageable>();
        animator = GetComponent<Animator>();
    }

    public void AddPotion(int amount)
    {
        currentPotions = Mathf.Clamp(currentPotions + amount, 0, maxPotions);
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


                if (pickupClip != null && audioSource != null)
                {
                    audioSource.PlayOneShot(pickupClip);
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
