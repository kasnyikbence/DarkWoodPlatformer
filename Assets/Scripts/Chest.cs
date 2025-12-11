using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{
    private GameObject player;
    private PlayerInput playerInput;
    private bool isOpen = false;
    Animator animator;
    private int amount;
    //private GameObject reward;


    void Start()
    {
        animator = GetComponent<Animator>();
        amount = Random.Range(1, 3);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player = col.gameObject;
            playerInput = player.GetComponent<PlayerInput>();

            if (playerInput != null)
            {
                playerInput.actions["ChestOpen"].performed -= OnInteract;
                playerInput.actions["ChestOpen"].performed += OnInteract;
            }

            if (!isOpen)
            {
                if (UIManager.Instance != null) UIManager.Instance.ShowInteractHint();
            }
            else
            {
                if (UIManager.Instance != null) UIManager.Instance.HideInteractHint();
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (playerInput != null)
            {
                playerInput.actions["ChestOpen"].performed -= OnInteract;
                playerInput = null;
                player = null;
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideInteractHint();
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isOpen)
        {
            PotionSystem potionSystem = player.GetComponent<PotionSystem>();

            if (potionSystem && potionSystem.currentPotions < potionSystem.maxPotions)
            {
                potionSystem.AddPotion(amount);
                UIManager.Instance.StartCoroutine(UIManager.Instance.ShowPickupMessage("+" + amount + " Potion"));
            }
            else
            {
                UIManager.Instance.StartCoroutine(UIManager.Instance.ShowPickupMessage("Potions are full!"));
            }

            isOpen = true;
            animator.SetBool("isOpen", true);
            UIManager.Instance.HideInteractHint();

            if (playerInput != null)
            {
                playerInput.actions["ChestOpen"].performed -= OnInteract;
            }
        }
    }
}
