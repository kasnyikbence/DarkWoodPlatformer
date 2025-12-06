using System;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    // Nem kell publikusnak lennie, mert a kód keresi meg
    private Camera cam;
    private Transform followTarget;

    Vector2 startingPosition;
    float startingZ;

    // Biztonságos hozzáférés a kamerához (ha esetleg null lenne)
    Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startingPosition;
    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;
    float clippigPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));
    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippigPlane;

    void Start()
    {
        // 1. Megkeressük az AKTUÁLIS fõkamerát
        cam = Camera.main;

        // 2. Megkeressük a JÁTÉKOST (tag alapján)
        // Fontos, mert a Player objektum is cserélõdhetett a scene váltáskor
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            followTarget = playerObj.transform;
        }
        else
        {
            Debug.LogError("ParallaxEffect: Nem található 'Player' taggel rendelkezõ objektum!");
            enabled = false; // Kikapcsoljuk a scriptet, hogy ne dobjon hibát
            return;
        }

        // 3. Kezdõpozíciók beállítása
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    void Update()
    {
        // Biztonsági ellenõrzés: Ha valamiért eltûnt a kamera vagy a célpont, ne fusson
        if (cam == null || followTarget == null) return;

        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}