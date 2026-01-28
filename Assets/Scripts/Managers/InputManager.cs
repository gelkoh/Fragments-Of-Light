using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class InputManager : SingletonManager
{
    public static InputManager Instance;

	private InputAction m_menuAction;

	private MenuManager m_menuManager;

	public event Action OnMenuActionPressed; 
    
    public override void InitializeManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

	private void Start()
	{
		m_menuManager = ManagersManager.Get<MenuManager>();

		InputActionAsset inputActions = InputSystem.actions;

		m_menuAction = inputActions.FindAction("Menu");
	}

	private void Update()
	{
		if (m_menuAction.WasPressedThisFrame())
		{
			OnMenuActionPressed?.Invoke();
		}
	}
}