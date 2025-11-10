using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public GameObject gameSavedTextPrefab;
    public Canvas gameCanves;
    [SerializeField] private TMP_Text potionCounterText;
    [SerializeField] private TMP_Text arrowCounterText;
    [SerializeField] private TMP_Text keyCounterText;


    [Header("Interact UI")]
    [SerializeField] private GameObject interactHintImage;

    void Awake()
    {
        Instance = this;
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

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanves.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanves.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void ShowGameSavedText(Vector3 position)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(position);

        Instantiate(gameSavedTextPrefab, spawnPosition, Quaternion.identity, gameCanves.transform);
    }
    public void ShowInteractHint()
    {
        interactHintImage.SetActive(true);
    }

    public void HideInteractHint()
    {
        interactHintImage.SetActive(false);
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
}
