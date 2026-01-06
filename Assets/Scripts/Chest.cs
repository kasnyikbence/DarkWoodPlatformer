using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{
    private GameObject player;
    private PlayerInput playerInput;
    private bool isOpen = false;
    Animator animator;

    private int potionAmount;
    private int arrowAmount;


    void Start()
    {
        animator = GetComponent<Animator>();
        potionAmount = Random.Range(1, 3);
        arrowAmount = Random.Range(1, 3);
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

            if (potionSystem != null)
            {
                string potionWord = "Potion";
                int spaceForPotions = potionSystem.maxPotions - potionSystem.currentPotions;

                if (spaceForPotions > 0)
                {
                    int amountToAdd = Mathf.Min(potionAmount, spaceForPotions);

                    potionSystem.AddPotion(amountToAdd);

                    if (amountToAdd > 1)
                    {
                        potionWord = "Potions";
                    }

                    UIManager.Instance.ShowPickupMessage("+" + amountToAdd + " " + potionWord);
                }
                else
                {
                    UIManager.Instance.ShowPickupMessage("Potions are full!");
                }
            }

            ProjectileLauncher projectileLauncher = player.GetComponent<ProjectileLauncher>();
            if (projectileLauncher != null)
            {
                string arrowWord = "Arrow";
                int spaceForarrows = projectileLauncher.maxArrows - projectileLauncher.currentArrows;

                if (spaceForarrows > 0)
                {
                    int amountToAdd = Mathf.Min(arrowAmount, spaceForarrows);

                    projectileLauncher.AddArrows(amountToAdd);

                    if (amountToAdd > 1)
                    {
                        arrowWord = "Arrows";
                    }
                    UIManager.Instance.ShowPickupMessage("+" + amountToAdd + " " + arrowWord);
                }
                else
                {
                    UIManager.Instance.ShowPickupMessage("Arrows are full!");
                }
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
