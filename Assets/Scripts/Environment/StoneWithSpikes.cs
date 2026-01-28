using UnityEngine;

public class StoneWithSpikes : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                player.Die();
            }
        }
    }
}
