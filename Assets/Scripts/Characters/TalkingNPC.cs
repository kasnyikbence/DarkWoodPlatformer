using UnityEngine;

public class TalkingNPC : NPC, ITalkable
{
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    public override void Interact()
    {
        StartDialogue();
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        dialogueController.DisplayNextParagraphs(dialogueText);
    }
}
