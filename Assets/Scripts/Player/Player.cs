using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player Instance;
    
	[SerializeField] public PlayerStats m_playerStats;
	[SerializeField] private AudioClip m_gameOverSound;
	
	private PlayerMovement m_movement;

    private void Awake()
    {
	    if (Instance != null && Instance != this)
	    {
		    Destroy(gameObject);
		    return;
	    }

	    Instance = this;
		m_movement = GetComponent<PlayerMovement>();
    }

	private void OnEnable()
	{
    	Book.OnPageFlip += HandlePageFlip;
		DialogueController.OnDialogueStarted += LockMovement;
        DialogueController.OnDialogueEnded += UnlockMovement;
	}

	private void OnDisable()
	{
    	Book.OnPageFlip -= HandlePageFlip;
		DialogueController.OnDialogueStarted -= LockMovement;
        DialogueController.OnDialogueEnded -= UnlockMovement;
	}

	public void Die()
	{
        StartCoroutine(DieRoutine());
    }

	private IEnumerator DieRoutine()
	{
    	m_movement.SetMovementLock(true);
    
    	ManagersManager.Get<SFXManager>().PlaySFXClip(m_gameOverSound, transform, 1f);

    	// 2. Teleport zum Checkpoint
    	Checkpoint lastCheckpoint = ManagersManager.Get<PlayerManager>().GetCheckpoint();
    	this.transform.position = lastCheckpoint.transform.position;

    	// 3. Eine Sekunde warten
    	yield return new WaitForSeconds(1f);

    	// 4. Bewegung wieder freigeben
    	m_movement.SetMovementLock(false);
	}

	private IEnumerator TemporaryMovementLock(float duration)
	{
    	m_movement.SetMovementLock(true);
    	yield return new WaitForSeconds(duration);
    	m_movement.SetMovementLock(false);
	}

	public void Save(ref PlayerSaveData playerSaveData)
	{
		playerSaveData.Position = transform.position;
	}

	public void Load(PlayerSaveData playerSaveData)
	{
	
		transform.position = playerSaveData.Position;
	}

	private void HandlePageFlip(PageID id)
	{
    	StartCoroutine(TemporaryMovementLock(1.5f)); 
	}
    
	private void LockMovement() => m_movement.SetMovementLock(true);
    private void UnlockMovement() => m_movement.SetMovementLock(false);
}

[System.Serializable]
public struct PlayerSaveData
{
	public Vector3 Position;
}