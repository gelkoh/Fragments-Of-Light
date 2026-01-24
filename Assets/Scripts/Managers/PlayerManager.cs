using UnityEngine;

public class PlayerManager : SingletonManager
{
    public static PlayerManager Instance;
    [SerializeField] private GameObject m_playerPrefab;
    private GameObject m_activePlayer;
    private Checkpoint m_lastCheckpoint = null;

    public override void InitializeManager()
    {
        Instance = this;
    }

    public GameObject EnsurePlayerExists()
    {
        if (m_activePlayer == null)
        {
            m_activePlayer = Instantiate(m_playerPrefab);
            
            // WICHTIG: Reset nach Instantiate
            PlayerMovement pm = m_activePlayer.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.ResetMovementState();
            }
        }

       return m_activePlayer;
    }

    public void DestroyPlayer()
    {
       if (m_activePlayer != null)
       {
          Destroy(m_activePlayer);
       }
    }

    public void TeleportPlayer(Vector3 position, Quaternion rotation)
    {
        if (m_activePlayer == null) return;
        
        // WICHTIG: Erst Movement State resetten
        PlayerMovement pm = m_activePlayer.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.ResetMovementState();
        }
        
        // Dann Position setzen
        m_activePlayer.transform.SetPositionAndRotation(position, rotation);
        
        // OPTIONAL: Noch ein Frame warten lassen
        // StartCoroutine(DelayedReset(pm));
    }
    
    // Optional: Für besonders hartnäckige Fälle
    private System.Collections.IEnumerator DelayedReset(PlayerMovement pm)
    {
        yield return new WaitForFixedUpdate();
        if (pm != null)
        {
            pm.ResetMovementState();
        }
    }
   
    public void SetCheckpoint(Checkpoint checkpoint)
    {
       m_lastCheckpoint = checkpoint;
    }

    public Checkpoint GetCheckpoint()
    {
       return m_lastCheckpoint;
    }

    public GameObject GetPlayer()
    {
       return m_activePlayer;
    }
}