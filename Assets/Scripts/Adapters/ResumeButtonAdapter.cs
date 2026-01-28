using UnityEngine;
using UnityEngine.UI; 

public class ResumeButtonAdapter : MonoBehaviour
{
    private Button m_button;
    private MenuManager m_menuManager;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleResumeButtonClicked);
        }

        m_menuManager = ManagersManager.Get<MenuManager>();
    }

    private void HandleResumeButtonClicked()
    {
        m_menuManager.HideMenu();
    }
}