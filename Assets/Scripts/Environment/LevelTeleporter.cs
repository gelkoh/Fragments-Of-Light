using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTeleporter : MonoBehaviour
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