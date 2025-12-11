using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    private GameObject player;
    private PlayerInput playerInput;
    private bool isOpen = false;
    Animator animator;
    BoxCollider2D doorCollider;
    BoxCollider2D triggerZone;



    void Start()
    {
        animator = GetComponent<Animator>();
        var colliders = GetComponents<BoxCollider2D>();
        triggerZone = colliders[0];
        doorCollider = colliders[1];

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player = col.gameObject;
            playerInput = player.GetComponent<PlayerInput>();

            if (playerInput != null)
            {
                playerInput.actions["DoorOpen"].performed -= OnInteract;
                playerInput.actions["DoorOpen"].performed += OnInteract;
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
                playerInput.actions["DoorOpen"].performed -= OnInteract;
            }
            UIManager.Instance.HideInteractHint();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isOpen) return;

        if (player == null)
        {
            return;
        }

        KeySystem inv = player.GetComponent<KeySystem>();
        if (inv == null)
        {
            return;
        }

        if (inv.UseKey())
        {
            OpenDoor();
        }
        else
        {
           // UIManager.Instance.ShowTemporaryMessage("Need a key to open."); // opcionális helper
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator?.SetBool("isOpen", true);

        if (doorCollider != null)
            doorCollider.enabled = false;

        UIManager.Instance.HideInteractHint();
    }

}
