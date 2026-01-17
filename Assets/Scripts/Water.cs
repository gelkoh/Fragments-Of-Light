using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                player.Die();
            }
        }
    }
}