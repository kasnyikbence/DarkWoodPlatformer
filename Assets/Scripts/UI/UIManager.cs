using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public GameObject gameSavedTextPrefab;
    public GameObject potionUI;
    public GameObject arrowUI;
    public GameObject keyUI;
    public Canvas gameCanvas;

    [SerializeField] private TMP_Text potionCounterText;
    [SerializeField] private TMP_Text arrowCounterText;
    [SerializeField] private TMP_Text keyCounterText;
    [SerializeField] private TMP_Text pickupMessageText;
    [SerializeField] private GameObject interactHintImage;
    private int interactHintCount = 0;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isDisplayingMessage = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

    }
    void OnEnable()
    {
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
    }

    void OnDisable()
    {
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
    }

    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void ShowGameSavedText(Vector3 position)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(position);

        Instantiate(gameSavedTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform);
    }

    public void ShowInteractHint()
    {
        interactHintCount++;
        if (interactHintCount > 0)
        {
            interactHintImage.SetActive(true);
        }
    }

    public void HideInteractHint()
    {
        interactHintCount--;
        if (interactHintCount <= 0)
        {
            interactHintCount = 0;
            interactHintImage.SetActive(false);
        }
    }

    public void UpdatePotionUI(int currentPotions)
    {
        potionCounterText.text = currentPotions.ToString();
    }

    public void UpdateArrowUI(int currentAmount)
    {
        arrowCounterText.text = currentAmount.ToString();
    }

    public void UpdateKeyUI(int keyAmount)
    {
        keyCounterText.text = keyAmount.ToString();
    }

    public void ShowPickupMessage(string message)
    {
        messageQueue.Enqueue(message);

        if (!isDisplayingMessage)
        {
            StartCoroutine(DisplayMessagesFromQueue());
        }
    }

    private IEnumerator DisplayMessagesFromQueue()
    {
        isDisplayingMessage = true;

        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();

            pickupMessageText.text = message;

            Color color = pickupMessageText.color;
            color.a = 1f;
            pickupMessageText.color = color;

            yield return new WaitForSeconds(1.0f);

            float fadeDuration = 0.5f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                pickupMessageText.color = color;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        pickupMessageText.text = "";
        isDisplayingMessage = false;
    }

    public void ShowPotionUI(bool show)
    {
        potionUI.gameObject.SetActive(show);
    }
    public void ShowArrowUI(bool show)
    {
        arrowUI.gameObject.SetActive(show);
    }
    public void ShowKeyUI(bool show)
    {
        keyUI.gameObject.SetActive(show);
    }
}
