using UnityEngine;

public class RotateSpinner : MonoBehaviour
{
    public float rotationSpeed;

    void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, Time.deltaTime * rotationSpeed);
    }
}
