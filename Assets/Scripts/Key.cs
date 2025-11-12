using UnityEngine;

public class Key : MonoBehaviour
{
    public float amplitude = 0.25f;
    public float speed = 1.5f;
    private Vector3 startPos;

    [SerializeField] private int keyAmount = 1;


    void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        KeySystem inv = collision.GetComponent<KeySystem>();
        if (inv != null)
        {
            inv.AddKey(keyAmount);
            UIManager.Instance.keyUI.SetActive(true);


            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Player has no KeySystem component!");
        }
    }

    void Update()
    {
        float yOffSet = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPos.x, startPos.y + yOffSet, startPos.z);
    }
}
