using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    MainMenuSettings,
    IngameMenu,
    Playing,
	MainMenuLoadGame
}

public class GameStateManager : SingletonManager
{
	public static GameStateManager Instance;

	private GameState m_currentGameState = GameState.MainMenu;

	public GameState CurrentGameState 
	{
   		get => m_currentGameState;
   		private set => m_currentGameState = value;
	}

	public static Action OnStart;
	public static Action OnEnd;
	
	public static Action<GameState> OnGameStateChanged;

	public override void InitializeManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

	public void StartGame()
	{
		OnStart?.Invoke();
		m_currentGameState = GameState.Playing;
	}

	public void EndGame()
	{
		OnEnd?.Invoke();
		m_currentGameState = GameState.MainMenu;
	}

	public void SetState(GameState newState)
	{
		m_currentGameState = newState;
		OnGameStateChanged?.Invoke(m_currentGameState);
	}

	public GameState GetState()
	{
		return m_currentGameState;
	}

	public void Quit()
	{
		#if UNITY_STANDALONE
					Application.Quit();
		#endif
		#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}