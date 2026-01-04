using UnityEngine;

public class Wraith : NPC, ITalkable
{
    [SerializeField] private DialogueText m_dialogueText;
    [SerializeField] private DialogueController m_dialogueController;
    
    public override void Interact()
    {
        Talk(m_dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        m_dialogueController.DisplayNextParagraph(dialogueText);
    }
}
