using UnityEngine;

public class AudioListenerFix : MonoBehaviour
{
    void Awake()
    {

        AudioListener[] listeners = FindObjectsByType<AudioListener>();

        if (listeners.Length > 1)
        {
            AudioListener myListener = GetComponent<AudioListener>();
            if (myListener != null)
            {
                myListener.enabled = false;
            }
        }
    }
}