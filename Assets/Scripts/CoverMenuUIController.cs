using UnityEngine;

public class CoverMenuUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject m_mainButtonsPanel;
    [SerializeField] private GameObject m_loadPanel;
    [SerializeField] private GameObject m_settingsPanel;

    private void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        m_mainButtonsPanel.SetActive(false);
        m_settingsPanel.SetActive(false);
        m_loadPanel.SetActive(false);

        switch (newState)
        {
            case GameState.MainMenu:
                m_mainButtonsPanel.SetActive(true);
                break;

            case GameState.MainMenuSettings:
                m_settingsPanel.SetActive(true);
                break;

            case GameState.MainMenuLoad:
                m_loadPanel.SetActive(true);
                break;
                
            case GameState.Playing:
                break;
        }
    }
}