using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour, IInteractable
{
    private GameObject player;
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
            UIManager.Instance.HideInteractHint();
        }
    }

    public void Interact()
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
           // TODO: Nincs kulcs kell valami üzenet
             Debug.Log("Nincs kulcsod a door kinyitásához!");
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
