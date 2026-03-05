using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance;

    private bool isStopped = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StopTime(float duration)
    {
        if (isStopped || Time.timeScale == 0f) return;

        StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        isStopped = true;

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        if (!PauseMenuManager.isPaused)
        {
            Time.timeScale = 1f;
        }

        isStopped = false;
    }
}