using UnityEngine;
using TMPro;

public class FlamesCollectedCounter : MonoBehaviour
{
    private PlayerManager m_playerManager;
    
    private void Awake()
    {
        m_playerManager = ManagersManager.Get<PlayerManager>();
    }
    
    private void OnEnable()
    {
        Player.OnFlameCountUpdated += HandleFlameCollected;
    }

    private void OnDisable()
    {
        Player.OnFlameCountUpdated -= HandleFlameCollected;
    }

    private void HandleFlameCollected()
    {
        Player player = m_playerManager.GetPlayer().GetComponent<Player>();
        
        GetComponent<TextMeshProUGUI>().text = player.flamesCollected.ToString();
    }
}
