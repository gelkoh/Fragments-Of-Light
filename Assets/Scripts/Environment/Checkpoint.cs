using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    void Awake()
    {
        // Hide the checkpoint when playing
        GetComponent<SpriteRenderer>().enabled = false;
    }
}