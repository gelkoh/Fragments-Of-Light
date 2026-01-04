using UnityEngine;

public class Flame : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            Debug.Log("Player collision with flame");
            Destroy(this);
        }
    }
}
