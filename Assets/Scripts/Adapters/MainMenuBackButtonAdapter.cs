using UnityEngine;
using UnityEngine.UI; 

public class MainMenuBackButtonAdapter : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleBackButtonClicked);
        }
    }

    private void HandleBackButtonClicked()
    {
        ManagersManager.Get<GameStateManager>().SetState(GameState.MainMenu);
    }
}