using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class BossTrigger : MonoBehaviour
{
    public BossController boss;
    public BossHealthBar bossHealthBar;
    public CinemachineCamera vCam;

    public EndScreenManager endScreenManager;

    public float targetOrthoSize = 8f;
    public float zoomSpeed = 2f;

    private float originalOrthoSize;
    private bool hasTriggered = false;
    private float delayBeforeVictory = 3f;

    void Start()
    {
        FindCameraReference();

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
            if (vCam == null)
            {
                FindCameraReference();
            }

            hasTriggered = true;

            if (boss != null) boss.ActivateBoss();
            if (bossHealthBar != null) bossHealthBar.Initialize(boss);

            if (vCam != null)
            {
                StopAllCoroutines();
                StartCoroutine(ChangeCameraSize(targetOrthoSize));
            }
            else
            {
                Debug.LogError("[BossTrigger] MÉG MINDIG nincs kamera! Ellenõrizd, hogy van-e 'CinemachineCamera' komponens a scene-ben (vagy a DDOL-ban)!");
            }

            GetComponent<Collider2D>().enabled = false;
        }
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

    IEnumerator ShowVictoryWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeVictory);
        endScreenManager.ShowVictory();
    }

    private void OnBossDefeated()
    {
        Debug.Log("[BossTrigger] Boss legyõzve -> Kamera visszaállítás.");

        if (vCam == null) FindCameraReference();

        if (vCam != null)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeCameraSize(originalOrthoSize));
        }

        if (endScreenManager != null)
        {
            StartCoroutine(ShowVictoryWithDelay());
        }
    }


}