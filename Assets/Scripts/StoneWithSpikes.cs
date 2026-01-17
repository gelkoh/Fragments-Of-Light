using UnityEngine;

public class StoneWithSpikes : MonoBehaviour
{
   private void OnTriggerStay2D(Collider2D other)
    {
        // CompareTag ist performanter als ein String-Vergleich
        if (other.CompareTag("Player")) 
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                player.Die();
            }
        }
    }
}
