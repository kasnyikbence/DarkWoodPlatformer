using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed = 10f;

    private Queue<string> paragraphs = new Queue<string>();
    private bool conversationEnded;
    private bool isTyping;
    [SerializeField] public static bool isPaused = false;
    private string p;
    private Coroutine typeDialogueCoroutine;
    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;
    public GameObject dialoguePanel;

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

        // Ha épp nem gépelünk, indítjuk a következõt
        if (!isTyping)
        {
            // Ellenõrzés, hogy van-e még a sorban (biztonsági okból)
            if (paragraphs.Count > 0)
            {
                p = paragraphs.Dequeue();
                typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
            }
        }
        else
        {
            // Ha gépelünk, befejezzük azonnal
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

            // Singleton hívás a UIManager felé
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideInteractHint();
            }

            isPaused = true;
        }

        NPCNameText.text = dialogueText.speakerName;

        // Queue feltöltése
        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }

    private void EndConversation()
    {
        conversationEnded = false;

        var npc = FindFirstObjectByType<NPC>();
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