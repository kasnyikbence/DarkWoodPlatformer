using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed = 10f;

    public AudioSource audioSource;
    public AudioClip dialogueBlipSound;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public int playAudioEveryNthChar = 2;

    private Queue<string> paragraphs = new Queue<string>();
    private bool conversationEnded;
    private bool isTyping;
    [SerializeField] public static bool isPaused = false;
    private string p;
    private Coroutine typeDialogueCoroutine;
    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;
    public GameObject dialoguePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        isPaused = false;
    }

    public void DisplayNextParagraphs(DialogueText dialogueText)
    {
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                StartConversation(dialogueText);
            }
            else if (conversationEnded && !isTyping)
            {
                EndConversation();
                return;
            }
        }

        if (!isTyping)
        {
            if (paragraphs.Count > 0)
            {
                p = paragraphs.Dequeue();
                typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
            }
        }
        else
        {
            FinishParagraphEarly();
        }

        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(DialogueText dialogueText)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideInteractHint();
            }

            isPaused = true;
        }

        NPCNameText.text = dialogueText.speakerName;

        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }

    private void EndConversation()
    {
        conversationEnded = false;

        var npc = FindAnyObjectByType<NPC>();
        if (npc != null)
            npc.EndDialogue();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

            isPaused = false;
    }

    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        NPCDialogueText.text = "";

        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;

        foreach (char c in p.ToCharArray())
        {
            alphaIndex++;
            NPCDialogueText.text = originalText;

            displayedText = NPCDialogueText.text.Insert(alphaIndex, HTML_ALPHA);
            NPCDialogueText.text = displayedText;

            if (c != ' ' && alphaIndex % playAudioEveryNthChar == 0)
            {
                if (audioSource != null && dialogueBlipSound != null)
                {
                    audioSource.pitch = Random.Range(minPitch, maxPitch);
                    audioSource.PlayOneShot(dialogueBlipSound);
                }
            }

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        if (typeDialogueCoroutine != null)
        {
            StopCoroutine(typeDialogueCoroutine);
        }

        NPCDialogueText.text = p;

        isTyping = false;
    }
}