using UnityEngine;

public class TutorialNPC : NPC, ITalkable
{
    [SerializeField] private DialogueText dialogueText;
    private DialogueController dialogueController;

    public override void Interact()
    {
        StartDialogue();
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.DisplayNextParagraphs(dialogueText);
        }
        else
        {
            Debug.LogError("TutorialNPC: Nem található DialogueController a jelenetben!");
        }
    }
}