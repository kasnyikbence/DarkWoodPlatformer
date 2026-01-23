using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{
    [Header("ID")]
    public string chestID;

    private GameObject player;
    private PlayerInput playerInput;
    private bool isOpen = false;
    Animator animator;

    private int potionAmountInChest;
    private int arrowAmountInChest;

    void Start()
    {
        animator = GetComponent<Animator>();

        potionAmountInChest = Random.Range(1, 3);
        arrowAmountInChest = Random.Range(1, 3);

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsChestOpen(chestID))
            {
                OpenChestVisualsOnly();
            }
        }

        if (string.IsNullOrEmpty(chestID))
        {
            Debug.LogWarning($"{gameObject.name} ládának nincs ID-ja! Nem lesz mentve az állapota.");
        }
    }

    private void OpenChestVisualsOnly()
    {
        isOpen = true;
        if (animator != null) animator.SetBool("isOpen", true);

        GetComponent<Collider2D>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isOpen) return;

        if (col.CompareTag("Player"))
        {
            player = col.gameObject;
            playerInput = player.GetComponent<PlayerInput>();

            if (playerInput != null)
            {
                playerInput.actions["ChestOpen"].performed -= OnInteract;
                playerInput.actions["ChestOpen"].performed += OnInteract;
            }

            if (UIManager.Instance != null) UIManager.Instance.ShowInteractHint();
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
            if (UIManager.Instance != null) UIManager.Instance.HideInteractHint();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isOpen)
        {
            PotionSystem potionSystem = player.GetComponent<PotionSystem>();
            if (potionSystem != null)
            {
                int space = potionSystem.MaxPotions - potionSystem.currentPotions;
                if (space > 0)
                {
                    int add = Mathf.Min(potionAmountInChest, space);
                    potionSystem.AddPotion(add);
                    if (UIManager.Instance) UIManager.Instance.ShowPickupMessage("+" + add + " Potion");
                    if (UIManager.Instance) UIManager.Instance.ShowPotionUI(true);
                }
                else
                {
                    if (UIManager.Instance) UIManager.Instance.ShowPickupMessage("Potions full!");
                }
            }

            ProjectileLauncher projectileLauncher = player.GetComponent<ProjectileLauncher>();
            if (projectileLauncher != null)
            {
                int space = projectileLauncher.MaxArrows - projectileLauncher.currentArrows;
                if (space > 0)
                {
                    int add = Mathf.Min(arrowAmountInChest, space);
                    projectileLauncher.AddArrows(add);
                    if (UIManager.Instance) UIManager.Instance.ShowPickupMessage("+" + add + " Arrow");
                    if (UIManager.Instance) UIManager.Instance.ShowArrowUI(true);
                }
                else
                {
                    if (UIManager.Instance) UIManager.Instance.ShowPickupMessage("Arrows full!");
                }
            }
            // ----------------------------------

            isOpen = true;
            if (animator != null) animator.SetBool("isOpen", true);

            if (UIManager.Instance != null) UIManager.Instance.HideInteractHint();

            if (GameManager.Instance != null && !string.IsNullOrEmpty(chestID))
            {
                GameManager.Instance.RegisterOpenedChest(chestID);
            }
            // -----------------------------------------

            if (playerInput != null)
            {
                playerInput.actions["ChestOpen"].performed -= OnInteract;
            }

            GetComponent<Collider2D>().enabled = false;
        }
    }
}