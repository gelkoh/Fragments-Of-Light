using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    void Awake()
    {
        //m_spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        // Hide the checkpoint when playing
        GetComponent<SpriteRenderer>().enabled = false;
    }
    
    /*public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            if (GameStateManager.Instance.GetCheckpoint() != this)
            {
                m_spriteRenderer.color = Color.white;
                GameStateManager.Instance.SetCheckpoint(this);
            }    
        }
    }*/
}