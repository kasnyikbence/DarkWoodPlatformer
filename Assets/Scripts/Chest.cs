using NUnit.Framework;
using System;
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
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }
        animator = GetComponent<Animator>();

        amount = Random.Range(1, 3);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (playerInput != null)
            {
                playerInput.actions["ChestOpen"].performed += OnInteract;
            }

            if (!isOpen)
            {
                UIManager.Instance.ShowInteractHint();
            }
            else
            {
                UIManager.Instance.HideInteractHint();
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
            }
            UIManager.Instance.HideInteractHint();
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
                UIManager.Instance.potionUI.SetActive(true);
            }
            else
            {
                UIManager.Instance.StartCoroutine(UIManager.Instance.ShowPickupMessage("Potions are full!"));
            }

            isOpen = true;
            animator.SetBool("isOpen", true);
            UIManager.Instance.HideInteractHint();

            playerInput.actions["ChestOpen"].performed -= OnInteract;

        }
    }
}
