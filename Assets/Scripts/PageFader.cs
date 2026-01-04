using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class PageFader : MonoBehaviour
{
    private CanvasGroup m_canvasGroup;

    private void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Book.OnPageFlip += HandlePageFlip;
    }
    
    private void OnDisable()
    {
        Book.OnPageFlip -= HandlePageFlip;
    }

    private void HandlePageFlip(PageID pageID)
    {
        if (pageID == PageID.Chapter1Level1Gameplay)
            StartCoroutine(FadeCoroutine());
    }
    
    private IEnumerator FadeCoroutine()
    {
        yield return new WaitForSeconds(10f);

        while (m_canvasGroup.alpha > 0f)
        {
            m_canvasGroup.alpha -= 0.005f;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
