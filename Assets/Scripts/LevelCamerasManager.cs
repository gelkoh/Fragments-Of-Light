using UnityEngine;
using System.Collections.Generic;

public class LevelCamerasManager : MonoBehaviour
{
    public List<LevelCameraController> levels;
	[SerializeField] private int m_forChapter;

    private int m_levelCameraIndex = 0;

    private void OnEnable()
    {
        Book.OnPageFlip += HandlePageFlip;
        GameStateManager.OnEnd += HandleEndGame;
    }
    
    private void OnDisable()
    {
        Book.OnPageFlip -= HandlePageFlip;
        GameStateManager.OnEnd -= HandleEndGame;
    }

    public void HandlePageFlip(PageID pageID)
    {
		string idString = pageID.ToString();

		Debug.Log("NEW PAGE ID: " + pageID);
		Debug.Log("idString: " + idString);
		Debug.Log("-----------------");

		if (!idString.Contains("Chapter" + m_forChapter) || !idString.Contains("Gameplay")) return; 

		if (pageID == PageID.Chapter1Level1Gameplay || pageID == PageID.Chapter2Level1Gameplay)
		{
			levels[0].Activate();
			return;
		}

        levels[m_levelCameraIndex].Deactivate();
        m_levelCameraIndex++;

        levels[m_levelCameraIndex].Activate();
    }

    private void HandleEndGame()
    {
	    m_levelCameraIndex = 0;
    }
}
