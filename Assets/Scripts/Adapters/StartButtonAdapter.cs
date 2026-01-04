using UnityEngine;

// Wichtig: Fügen Sie UnityEngine.UI hinzu, um mit Buttons zu arbeiten
using UnityEngine.UI; 

public class StartButtonAdapter : MonoBehaviour
{
    private Button m_button;

    void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleStartGame);
        }
    }

    private void HandleStartGame()
    {
        GameStateManager gameStateManager = ManagersManager.Get<GameStateManager>();
        
        if (gameStateManager != null)
        {
            gameStateManager.StartGame();
        }
        else
        {
            Debug.LogError("GameStateManager not found or not initialized!");
        }
        
        m_button.interactable = false;
    }
}