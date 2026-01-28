using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void Awake()
    {
        // Hide the checkpoint when playing
        GetComponent<SpriteRenderer>().enabled = false;
    }
}