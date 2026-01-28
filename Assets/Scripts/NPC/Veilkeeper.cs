using UnityEngine;
using System.Collections;

public class Veilkeeper : MonoBehaviour, IInteractable
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Interact();
        }
    }

    public void Interact()
    {
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        
        while (transform.localPosition.x < 35f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + 0.05f, transform.localPosition.y, transform.localPosition.z);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
