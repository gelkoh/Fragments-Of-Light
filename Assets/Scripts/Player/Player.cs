using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance;
    
	[SerializeField] public PlayerStats m_playerStats;
	[SerializeField] private AudioClip m_gameOverSound;

	public int flamesCollected = 0;
	public static Action OnFlameCountUpdated;
	
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
		Flame.OnFlameCollected += HandleFlameCollected;
	}

	private void OnDisable()
	{
    	Book.OnPageFlip -= HandlePageFlip;
		DialogueController.OnDialogueStarted -= LockMovement;
        DialogueController.OnDialogueEnded -= UnlockMovement;		
		Flame.OnFlameCollected -= HandleFlameCollected;
	}

	private void Start()
	{
		StartCoroutine(TemporaryMovementLock(12f)); 
	}

	public void Die()
	{
        StartCoroutine(DieRoutine());
    }

	private IEnumerator DieRoutine()
	{
    	m_movement.SetMovementLock(true);
    
    	ManagersManager.Get<SFXManager>().PlaySFXClip(m_gameOverSound, transform, 1f);

    	Checkpoint lastCheckpoint = ManagersManager.Get<PlayerManager>().GetCheckpoint();
    	this.transform.position = lastCheckpoint.transform.position;

    	yield return new WaitForSeconds(1f);

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

	private void HandlePageFlip(PageID pageID)
	{
    	StartCoroutine(TemporaryMovementLock(1.2f)); 
	}
    
	private void LockMovement() => m_movement.SetMovementLock(true);
    private void UnlockMovement() => m_movement.SetMovementLock(false);

	private void HandleFlameCollected()
	{
		flamesCollected++;
		Debug.Log("Flames collected: " + flamesCollected);
		OnFlameCountUpdated?.Invoke();
	}
}

[System.Serializable]
public struct PlayerSaveData
{
	public Vector3 Position;
}