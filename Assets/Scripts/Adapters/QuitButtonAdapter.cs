using UnityEngine;
using UnityEngine.UI; 

public class QuitButtonAdapter : MonoBehaviour
{
    private Button m_button;
    private GameStateManager m_gameStateManager;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleQuitButtonClicked);
        }

        m_gameStateManager = ManagersManager.Get<GameStateManager>();
    }

    private void HandleQuitButtonClicked()
    {
        m_gameStateManager.Quit();
    }
}