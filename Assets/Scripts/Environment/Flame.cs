using UnityEngine;
using System;

public class Flame : MonoBehaviour
{
    public static Action OnFlameCollected;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            OnFlameCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
