using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoint : MonoBehaviour
{
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = true;
            UIManager.Instance.ShowInteractHint();
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = false;
            UIManager.Instance.HideInteractHint();
        }
    }
     public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && playerInRange)
        {
            SaveCheckpoint();
        }
    }
    private void SaveCheckpoint()
    {
        // ideiglenes
        PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
        PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
        PlayerPrefs.SetFloat("CheckpointZ", transform.position.z);

        PlayerPrefs.Save();

        Debug.Log("Checkpoint elmentve: " + transform.position);
    }
}
