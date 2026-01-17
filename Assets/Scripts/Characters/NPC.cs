using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NPC : MonoBehaviour, IInteractable
{

    private Transform _playerTransform;
    private const float INTERACT_DISTANCE = 2f;
    protected bool isTalking = false;
    private bool wasWithinInteractDistance = false;


    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void Update()
    {
        if (_playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _playerTransform = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        bool currentlyWithinDistance = IsWithinInteractDistance();

        if (Keyboard.current.eKey.wasPressedThisFrame && currentlyWithinDistance)
        {
            Interact();
        }

        if (!isTalking)
        {
            if (currentlyWithinDistance && !wasWithinInteractDistance)
            {
                UIManager.Instance.ShowInteractHint();
            }
            else if (!currentlyWithinDistance && wasWithinInteractDistance)
            {
                UIManager.Instance.HideInteractHint();
            }
        }

        wasWithinInteractDistance = currentlyWithinDistance;
    }
    
    public abstract void Interact();

    protected void StartDialogue()
    {
        isTalking = true;
        UIManager.Instance.HideInteractHint(); 
    }

    public void EndDialogue()
    {
        isTalking = false;

        if (IsWithinInteractDistance())
        {
            UIManager.Instance.ShowInteractHint();
        }
    }


    private bool IsWithinInteractDistance()
    {
        if (_playerTransform == null) return false;

        if (Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE)
        {
            FacePlayer();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FacePlayer()
    {
        if (_playerTransform.position.x < transform.position.x) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);
    }
}
