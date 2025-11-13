using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DialogueController : MonoBehaviour
{
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
                p = paragraphs.Dequeue();

                typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
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
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            UIManager.Instance.HideInteractHint();
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

        var npc = FindFirstObjectByType<NPC>();
        if (npc != null)
            npc.EndDialogue();

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
        isPaused = false;
    }
    //Régi kiíratás
    
    /*
    private IEnumerator TypeDialogueText(string p)
    {
        float elapsedTime = 0f;

        int charIndex = 0;
        charIndex = Mathf.Clamp(charIndex, 0, p.Length);

        while (charIndex < p.Length)
        {
            elapsedTime += Time.deltaTime * typeSpeed;
            charIndex = Mathf.FloorToInt(elapsedTime);

            NPCDialogueText.text = p.Substring(0, charIndex);

            yield return null;
        }

        NPCDialogueText.text = p;
    }
    */
    
    
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
        StopCoroutine(typeDialogueCoroutine);

        NPCDialogueText.text = p;

        isTyping = false;
    }

    

}
