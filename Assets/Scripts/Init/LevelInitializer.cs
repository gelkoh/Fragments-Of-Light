using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private Checkpoint m_associatedCheckpoint;

    public void SpawnPlayerHere()
    {
        PlayerManager playerManager = ManagersManager.Get<PlayerManager>();
        playerManager.EnsurePlayerExists();
        
        playerManager.TeleportPlayer(
            m_associatedCheckpoint.transform.position, 
            m_associatedCheckpoint.transform.rotation
        );
        
        playerManager.SetCheckpoint(m_associatedCheckpoint);
    }
}