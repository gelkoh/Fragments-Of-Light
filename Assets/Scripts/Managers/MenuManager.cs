using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : SingletonManager
{
    public static MenuManager Instance;
    
    [SerializeField] private GameObject m_menuPrefab;
	
	[SerializeField] private BookSettings m_bookSettings;

    [SerializeField] private GameObject m_menu;
    
    private InputManager m_inputManager;
    private GameStateManager m_gameStateManager;

    public override void InitializeManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
    }
    
    private void Start()
    {
        m_inputManager = ManagersManager.Get<InputManager>();
        m_gameStateManager = ManagersManager.Get<GameStateManager>();
            
        m_inputManager.OnMenuActionPressed += HandleMenuActionPressed;
    }

    private void OnDisable()
    {
        m_inputManager.OnMenuActionPressed -= HandleMenuActionPressed;
    }

    private void HandleMenuActionPressed()
    {
        if (m_gameStateManager.CurrentGameState == GameState.MainMenu) return;
        
        if (m_gameStateManager.CurrentGameState == GameState.IngameMenu)
        {
            HideMenu();
        }
        else
        {

            ShowMenu();
        }
    }

	private void ShowMenu()
	{
        m_gameStateManager.SetState(GameState.IngameMenu);
        Book.Instance.ShowMenuOnCurrentPageLeft(m_bookSettings.IngameMenuLeftMaterial);
        Book.Instance.ShowMenuOnCurrentPageRight(m_bookSettings.IngameMenuRightMaterial);

   		Time.timeScale = 0.0f;
	}

	public void HideMenu()
	{
        m_gameStateManager.SetState(GameState.Playing);
        Book.Instance.HideMenuOnCurrentPageLeft();
        Book.Instance.HideMenuOnCurrentPageRight();

		Time.timeScale = 1.0f;
	}
}