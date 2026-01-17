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
        if (m_activePlayer != null)
        {
            m_activePlayer.transform.SetPositionAndRotation(position, rotation);
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