using UnityEngine;

public class KeySystem : MonoBehaviour
{
    [SerializeField] private int keyCount = 0;

    public int KeyCount => keyCount;

    public void AddKey(int amount = 1)
    {
        keyCount = Mathf.Max(0, keyCount + amount);
       UIManager.Instance.UpdateKeyUI(keyCount);
        Debug.Log($"Key added. Now: {keyCount}");
    }

    public bool UseKey()
    {
        if (keyCount > 0)
        {
            keyCount--;
            UIManager.Instance.UpdateKeyUI(keyCount);
            Debug.Log($"Key used. Remaining: {keyCount}");
            return true;
        }

        Debug.Log("No keys to use.");
        return false;
    }
}
