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
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerInput = player.GetComponent<PlayerInput>();

        animator = GetComponent<Animator>();
        var colliders = GetComponents<BoxCollider2D>();
        triggerZone = colliders[0];
        doorCollider = colliders[1];

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (playerInput != null)
            {
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
            Debug.LogWarning("Door: player nincs beállítva.");
            return;
        }

        KeySystem inv = player.GetComponent<KeySystem>();
        if (inv == null)
        {
            Debug.LogWarning("Door: playernek nincs KeyInventory komponense.");
            return;
        }

        // Próbálunk kulcsot használni. Ha van, UseKey() true-t ad vissza.
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

        // Fizikai blokk kikapcsolása — így át tudsz menni rajta
        if (doorCollider != null)
            doorCollider.enabled = false;

        UIManager.Instance.HideInteractHint();
    }

}
