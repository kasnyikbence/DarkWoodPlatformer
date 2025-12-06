using UnityEngine;

public class TutorialNPC : NPC, ITalkable
{
    [SerializeField] private DialogueText dialogueText;
    // Töröljük a SerializeField-et, vagy nem használjuk közvetlenül
    private DialogueController dialogueController;

    public override void Interact()
    {
        StartDialogue();
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        // Dinamikusan keressük meg, ha nincs meg, vagy ha a régi megsemmisült
        if (dialogueController == null)
        {
            dialogueController = FindFirstObjectByType<DialogueController>();
        }

        if (dialogueController != null)
        {
            dialogueController.DisplayNextParagraphs(dialogueText);
        }
        else
        {
            Debug.LogError("TutorialNPC: Nem található DialogueController a jelenetben!");
        }
    }
}