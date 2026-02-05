using System.Collections;
using UnityEngine;
using Unity.Cinemachine; // Unity 6 / Cinemachine 3.x

public class BossTrigger : MonoBehaviour
{
    [Header("Referenciák")]
    public BossController boss;
    public BossHealthBar bossHealthBar;
    public CinemachineCamera vCam;

    [Header("Kamera Beállítások")]
    public float targetOrthoSize = 14f;
    public float zoomSpeed = 2f;

    private float originalOrthoSize;
    private bool hasTriggered = false;

    void Start()
    {
        if (vCam != null)
        {
            originalOrthoSize = vCam.Lens.OrthographicSize;
        }

        if (boss != null)
        {
            boss.OnBossDeath += OnBossDefeated;
        }
    }

    void OnDestroy()
    {
        if (boss != null)
        {
            boss.OnBossDeath -= OnBossDefeated;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;

            if (boss != null) boss.ActivateBoss();
            if (bossHealthBar != null) bossHealthBar.Initialize(boss);

            if (vCam != null)
            {
                StopAllCoroutines();
                StartCoroutine(ChangeCameraSize(targetOrthoSize));
            }

            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnBossDefeated()
    {
        Debug.Log("[BossTrigger] Boss legyõzve -> Kamera visszaállítás.");
        if (vCam != null)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeCameraSize(originalOrthoSize));
        }
    }

    IEnumerator ChangeCameraSize(float targetSize)
    {
        var lensSettings = vCam.Lens;
        float startSize = lensSettings.OrthographicSize;
        float currentSize = startSize;

        while (Mathf.Abs(currentSize - targetSize) > 0.05f)
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * zoomSpeed);

            lensSettings.OrthographicSize = currentSize;
            vCam.Lens = lensSettings;

            yield return null;
        }

        yield return new WaitForSeconds(3f);

        lensSettings.OrthographicSize = targetSize;
        vCam.Lens = lensSettings;
    }
}