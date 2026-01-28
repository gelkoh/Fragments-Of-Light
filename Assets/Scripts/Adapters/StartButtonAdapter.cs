using UnityEngine;

// Wichtig: Fügen Sie UnityEngine.UI hinzu, um mit Buttons zu arbeiten
using UnityEngine.UI; 

public class StartButtonAdapter : MonoBehaviour
{
    private Button m_button;
    private GameStateManager m_gameStateManager;

    void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleStartGame);
        }

        m_gameStateManager = ManagersManager.Get<GameStateManager>();
    }

    private void OnEnable()
    {
        GameStateManager.OnEnd += HandleEndGame;
    }
    
    private void OnDisable()
    {
        GameStateManager.OnEnd -= HandleEndGame;
    }

    private void HandleStartGame()
    {
        m_gameStateManager.StartGame();
        
        m_button.interactable = false;
    }

    private void HandleEndGame()
    {
        m_button.interactable = true;
    }
}