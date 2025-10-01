using UnityEngine;
using UnityEngine.InputSystem;

public class PotionSystem : MonoBehaviour
{
    public int maxPotions = 3;
    public int currentPotions;

    public int healAmount = 20;

    private Damageable playerDamageable;

    [Header("Audio")]
    public AudioClip pickupClip;
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerDamageable = GetComponent<Damageable>();
        currentPotions = maxPotions;
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
