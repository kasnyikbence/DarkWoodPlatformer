using UnityEngine;

public class AnimationEventHelper : MonoBehaviour
{
    private SceneChanger sceneChanger;

    void Start()
    {
        sceneChanger = GetComponentInParent<SceneChanger>();
        if (sceneChanger == null)
        {
            Debug.LogError("AnimationEventHelper HIBA: SceneChanger szkript nem található a szülõ objektumokon!");
        }
    }

    public void CallOnFadeComplete()
    {
        if (sceneChanger != null)
        {
            sceneChanger.OnFadeComplete();
        }
    }
}