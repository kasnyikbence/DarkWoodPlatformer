using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NPC : MonoBehaviour, IInteractable
{

    private Transform _playerTransform;
    private const float INTERACT_DISTANCE = 2f;
    protected bool isTalking = false;
    private bool wasWithinInteractDistance = false;
    private Vector3 originalScale;

    public CinemachineCamera vCam;
    public const float TARGET_ORTHO_SIZE = 6f;
    public float zoomSpeed = 2f;

    private float originalOrthoSize;


    private void Start()
    {
        FindCameraReference();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        originalScale = transform.localScale;
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
        StartCoroutine(ChangeCameraSize(TARGET_ORTHO_SIZE));


    }

    public void EndDialogue()
    {
        isTalking = false;
        StartCoroutine(ChangeCameraSize(originalOrthoSize));

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
        if (_playerTransform.position.x < transform.position.x) transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        else transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }

    private void FindCameraReference()
    {
        if (vCam == null)
        {
            vCam = FindAnyObjectByType<CinemachineCamera>(FindObjectsInactive.Include);
        }

        if (vCam != null && originalOrthoSize == 0)
        {
            originalOrthoSize = vCam.Lens.OrthographicSize;
        }
    }

    IEnumerator ChangeCameraSize(float targetSize)
    {
        var lensSettings = vCam.Lens;
        float currentSize = lensSettings.OrthographicSize;

        while (Mathf.Abs(currentSize - targetSize) > 0.05f)
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * zoomSpeed);

            lensSettings.OrthographicSize = currentSize;
            vCam.Lens = lensSettings;

            yield return null;
        }

        lensSettings.OrthographicSize = targetSize;
        vCam.Lens = lensSettings;
    }
}
