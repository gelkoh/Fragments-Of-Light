using UnityEngine;
using UnityEngine.UI; 

public class LoadGameButtonAdapter : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleLoadButtonClicked);
        }
    }

    private void HandleLoadButtonClicked()
    {
        ManagersManager.Get<GameStateManager>().SetState(GameState.MainMenuLoadGame);
    }
}