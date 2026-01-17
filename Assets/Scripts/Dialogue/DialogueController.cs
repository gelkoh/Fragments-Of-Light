using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_npcNameText;
    [SerializeField] private TextMeshProUGUI m_npcDialogueText;
    [SerializeField] private float m_typeSpeed = 10f;
    
    private Queue<string> m_paragraphs = new Queue<string>();
    private bool m_conversationEnded;
    private string m_p;
    private Coroutine m_typeDialogueCoroutine;
    private bool m_isTyping;
    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;
    
    public static Action OnDialogueStarted;
    public static Action OnDialogueEnded;

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        if (m_paragraphs.Count == 0)
        {
            if (!m_conversationEnded)
            {
                StartConversation(dialogueText);
            }
            else if (m_conversationEnded && !m_isTyping)
            {
                EndConversation();
                return;
            }
        }

        if (!m_isTyping)
        {
            m_p = m_paragraphs.Dequeue();
            m_typeDialogueCoroutine = StartCoroutine(TypeDialogueText(m_p));
        }
        else
        {
            FinishParagraphEarly();
        }

        if (m_paragraphs.Count == 0)
        {
            m_conversationEnded = true;
        }
    }

    private void StartConversation(DialogueText dialogueText)
    {
        OnDialogueStarted?.Invoke();
        
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        m_npcNameText.text = dialogueText.speakerName;

        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            m_paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }

    private void EndConversation()
    {
        m_conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        
        OnDialogueEnded?.Invoke();
    }

    private IEnumerator TypeDialogueText(string p)
    {
        m_isTyping = true;

        m_npcDialogueText.text = "";

        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;

        foreach (char c in p.ToCharArray())
        {
            alphaIndex++;
            m_npcDialogueText.text = originalText;

            displayedText = m_npcDialogueText.text.Insert(alphaIndex, HTML_ALPHA);
            m_npcDialogueText.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_TIME / m_typeSpeed);
        }
        
        m_isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        StopCoroutine(m_typeDialogueCoroutine);

        m_npcDialogueText.text = m_p;
        
        m_isTyping = false;
    }
}