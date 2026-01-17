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

	//private CanvasGroup m_menuCanvasGroup;

    public override void InitializeManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
    }
    
    private void Awake()
    {
        //m_menuCanvasGroup = m_menu.GetComponent<CanvasGroup>();

		//HideMenu();
    }

    private void Start()
    {
        //SceneManager.MoveGameObjectToScene(m_menu, SceneManager.GetSceneByName("PagesScene"));
        
        m_inputManager = ManagersManager.Get<InputManager>();
        m_gameStateManager = ManagersManager.Get<GameStateManager>();
            
        m_inputManager.OnMenuActionPressed += HandleMenuActionPressed;
    }

    private void OnDisable()
    {
        m_inputManager.OnMenuActionPressed -= HandleMenuActionPressed;
    }

    /*private void HandleMenuActionPressed()
    {
        if (m_gameStateManager.CurrentGameState == GameState.MainMenu)
            return;
        
        if (m_gameStateManager.CurrentGameState == GameState.IngameMenu)
        {
            m_gameStateManager.SetState(GameState.Playing);
			
			HideMenu();
        }
        else
        {
            m_gameStateManager.SetState(GameState.IngameMenu);

			ShowMenu();
        }
    }*/

private void HandleMenuActionPressed()
{
	Debug.Log("Handle menu action pressed");
    if (m_gameStateManager.CurrentGameState == GameState.MainMenu) return;
    
    if (m_gameStateManager.CurrentGameState == GameState.IngameMenu)
    {
        HideMenu(); // Blendet das Overlay-Canvas (alpha=0) aus
    }
    else
    {

        ShowMenu(); // Setzt Alpha auf 1, damit die RenderTexture befüllt wird
    }
}

	public void HideMenu()
	{
        m_gameStateManager.SetState(GameState.Playing);
        Book.Instance.HideMenuOnCurrentPage();
        Book.Instance.HideMenuOnCurrentPageRight();

		/*m_menuCanvasGroup.alpha = 0;
		m_menuCanvasGroup.interactable = false;
		m_menuCanvasGroup.blocksRaycasts = false;*/
		Time.timeScale = 1.0f;
	}

	private void ShowMenu()
	{
        m_gameStateManager.SetState(GameState.IngameMenu);
        Book.Instance.ShowMenuOnCurrentPage(m_bookSettings.IngameMenuLeftMaterial);
        Book.Instance.ShowMenuOnCurrentPageRight(m_bookSettings.IngameMenuRightMaterial);


		/*m_menuCanvasGroup.alpha = 1;
		m_menuCanvasGroup.interactable = true;
		m_menuCanvasGroup.blocksRaycasts = true;*/
   		Time.timeScale = 0.0f;
	}
}