using UnityEngine;
using System.Collections;
using TMPro;

public class TextAnimator : MonoBehaviour
{
    [SerializeField] private float m_animationSpeed;
    
    private TMP_Text m_textComponent;
    private string m_text = "";
    private int m_letterIndex = 0;
    
    void Awake()
    {
        m_textComponent = this.gameObject.GetComponent<TextMeshProUGUI>();

        m_text = m_textComponent.text;

        m_textComponent.text = "";
    }

    void Start()
    {
        StartCoroutine(StartTextAnimation());
    }

    private IEnumerator StartTextAnimation()
    {
        while (m_letterIndex < m_text.Length)
        {
            m_textComponent.text += m_text[m_letterIndex];
            m_letterIndex++;
            
            yield return new WaitForSeconds(m_animationSpeed);
        }
    }
}