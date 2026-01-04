using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue")]
public class DialogueText : ScriptableObject
{
    public string speakerName;
    
    [TextArea(5, 10)]
    public string[] paragraphs;
}