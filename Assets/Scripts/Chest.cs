using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    private GameObject player;
    private PlayerInput playerInput;
    private bool isOpen = false;
    Animator animator;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }
        animator = GetComponent<Animator>();

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
            isOpen = true;
            animator.SetBool("isOpen", true);
            UIManager.Instance.HideInteractHint();
        }
    }
}
