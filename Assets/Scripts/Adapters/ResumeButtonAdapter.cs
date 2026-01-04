using UnityEngine;
using UnityEngine.UI; 

public class ResumeButtonAdapter : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleResumeButtonClicked);
        }
    }

    private void HandleResumeButtonClicked()
    {
        ManagersManager.Get<GameStateManager>().SetState(GameState.Playing);
    }
}