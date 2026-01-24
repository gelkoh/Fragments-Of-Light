using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter2D()
    {
        Book.Instance.FlipPage();
        Destroy(gameObject);
    }

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}