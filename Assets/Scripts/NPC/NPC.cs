using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer m_interactSprite;
    private const float INTERACT_DISTANCE = 1f;

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && IsWithinInteractDistance())
        {
            Interact();
        }

        if (m_interactSprite.gameObject.activeSelf && !IsWithinInteractDistance())
        {
            m_interactSprite.gameObject.SetActive(false);
        }
        else if (!m_interactSprite.gameObject.activeSelf && IsWithinInteractDistance())
        {
            m_interactSprite.gameObject.SetActive(true);
        }
    }

    public abstract void Interact();

    private bool IsWithinInteractDistance()
    {
		if (Player.Instance == null) return false;

        Transform m_playerTransform = Player.Instance.transform;

        if (Vector2.Distance(m_playerTransform.position, transform.position) < INTERACT_DISTANCE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
