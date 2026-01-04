using UnityEngine;
using UnityEngine.UI; 

public class MainMenuButtonAdapter : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(GoToMainMenu);
        }
    }

    private void GoToMainMenu()
    {
        ManagersManager.Get<GameStateManager>().SetState(GameState.MainMenu);
    }
}