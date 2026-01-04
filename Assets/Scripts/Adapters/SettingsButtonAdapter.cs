using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonAdapter : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleSettingsButtonClicked);
        }
    }

    private void HandleSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked");
        ManagersManager.Get<GameStateManager>().SetState(GameState.MainMenuSettings);
    }
}