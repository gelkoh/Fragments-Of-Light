using UnityEngine;
using System.Collections;

public class Veilkeeper : MonoBehaviour
{
    /*private void OnEnable()
    {
        Book.OnPageFlip += HandlePageFliBp;
    }
    private void OnDisable()
    {
        Book.OnPageFlip -= HandlePageFlip;
    }

    private void HandlePageFlip(PageID newPage)
    {
        
    }*/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("PLAYER AND VEILKEEPER COLLIDED");
            StartCoroutine(Run());
        }
    }

    private IEnumerator Run()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        
        while (transform.localPosition.x < 35f)
        {
            Debug.Log("LOCAL POS: " + transform.localPosition);
            transform.localPosition = new Vector3(transform.localPosition.x + 0.05f, transform.localPosition.y, transform.localPosition.z);
            yield return new WaitForSeconds(0.02f);
        }
    }
    
    /*private IEnumerator FadeCoroutine()
    {
        yield return new WaitForSeconds(6f);

        while (m_canvasGroup.alpha > 0f)
        {
            m_canvasGroup.alpha -= 0.005f;
            yield return new WaitForSeconds(0.02f);
        }
    }*/
}
