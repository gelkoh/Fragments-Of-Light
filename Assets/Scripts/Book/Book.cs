using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class Book : MonoBehaviour
{
	private bool m_useDebugStart = false;
	private PageID m_debugStartPage = PageID.Chapter2Level4Gameplay;





	public static Book Instance;
    
	[SerializeField] private BookSettings m_bookSettings;

    private int m_currentPageIndex = 0;

	public static Action<PageID> OnPageFlip;

	private PageID m_currentPage = PageID.CoverFront;

	public PageID CurrentPage
	{
		get => m_currentPage;
		set => m_currentPage = value;
	}

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

    void OnEnable()
    {
		GameStateManager.OnStart += HandleStartGame;
    }

    void OnDisable()
    {
		GameStateManager.OnStart -= HandleStartGame;
    }
    
    void Start()
    {
		int counter = 0;		

		foreach (Transform child in this.gameObject.transform)
		{
			child.gameObject.AddComponent<PageFlipper>();
			child.gameObject.AddComponent<MeshCollider>();

			if (counter == 0)
			{
				ApplyPageTextures(child, m_bookSettings.CoverFrontMaterial, m_bookSettings.Endpaper1LeftMaterial);
				AddPageClickDetector(child, "CoverUICanvas");
			}

			if (counter == 1)
			{
				ApplyPageTextures(child, m_bookSettings.Endpaper1RightMaterial, m_bookSettings.FrontispieceMaterial);
				AddPageClickDetector(child, "Endpaper1UICanvas");
			}

			if (counter == 2)
			{
				ApplyPageTextures(child, m_bookSettings.TitlepageMaterial, m_bookSettings.Chapter1IntroductionLeftMaterial);
				AddPageClickDetector(child, "FrontispieceAndTitlepageUICanvas");
			}

            if (counter == 3)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter1IntroductionRightMaterial, m_bookSettings.Chapter1Level1LeftMaterial);
				AddPageClickDetector(child, "Chapter1IntroductionUICanvas");
			}

			if (counter == 4)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter1Level1RightMaterial, m_bookSettings.Chapter1Level2LeftMaterial);
			}

			if (counter == 5)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter1Level2RightMaterial, m_bookSettings.Chapter2IntroductionLeftMaterial);
			}

			if (counter == 6)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2IntroductionRightMaterial, m_bookSettings.Chapter2Level1LeftMaterial);
				AddPageClickDetector(child, "Chapter2IntroductionUICanvas");
			}

			if (counter == 7)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2Level1RightMaterial, m_bookSettings.Chapter2Level2LeftMaterial);
			}

			if (counter == 8)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2Level2RightMaterial, m_bookSettings.Chapter2Level3LeftMaterial);
			}

			if (counter == 9)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2Level3RightMaterial, m_bookSettings.Chapter2Level4LeftMaterial);
			}

			if (counter == 10)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2Level4RightMaterial, m_bookSettings.ThanksForPlayingThePrototypeLeftMaterial);
			}

			if (counter == 11)
			{
				ApplyPageTextures(child, m_bookSettings.ThanksForPlayingThePrototypeRightMaterial, m_bookSettings.Endpaper2LeftMaterial);
				AddPageClickDetector(child, "ThanksForPlayingThePrototypeUICanvas");
			}

			if (counter == 12)
			{
				ApplyPageTextures(child, m_bookSettings.Endpaper2RightMaterial, m_bookSettings.CoverBackMaterial);
				AddPageClickDetector(child, "Endpaper2UICanvas");
			}

			if (counter == transform.childCount - 1)
			{
				ApplyPageTextures(child, m_bookSettings.Endpaper2RightMaterial, m_bookSettings.CoverBackMaterial);
			}

			counter++;
		}
    }

	private void HandleStartGame()
	{
    	if (m_useDebugStart)
    	{
        	StartCoroutine(DebugStartRoutine());
    	}
    	else
    	{
        	StartCoroutine(MoveBookToRight());
        	FlipPage();
    	}
	}

	private IEnumerator MoveBookToRight()
	{
    	float duration = 0.6f;
    	float elapsed = 0f;
        Vector3 startPos = transform.localPosition;
        // Wir erzwingen Y und Z auf ihre Ursprungswerte (z.B. 0 und -0.25)
        Vector3 targetPos = new Vector3(2.5f, 0f, -0.9f); 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            
            // Lerp statt Translate garantiert, dass wir auf der Schiene bleiben
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        transform.localPosition = targetPos;
    }

// Neue Version mit Parameter, um zwischen Start-Mitte und Ende-Mitte zu unterscheiden
private IEnumerator MoveBookToCenter(bool isEnding = false)
{
    float duration = 0.6f;
    float elapsed = 0f;
    Vector3 startPos = transform.localPosition;
    
    // Wenn wir am Ende sind (isEnding), muss X auf 5.0f (oder dein gemessener Wert),
    // damit das Backcover wieder optisch in der Mitte landet.
    float targetX = isEnding ? 5.0f : 0f; 
    Vector3 targetPos = new Vector3(targetX, 0f, 0f); 

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.SmoothStep(0, 1, elapsed / duration);
        transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
        yield return null;
    }

    transform.localPosition = targetPos;
}

    public void FlipPage()
    {
		if (m_currentPage == PageID.Endpaper2)
		{
    		CloseBookInstantly();
			GameStateManager gameStateManager = ManagersManager.Get<GameStateManager>();
			gameStateManager.EndGame();
			return;
		}

		SFXManager.Instance.PlaySFXClip(m_bookSettings.PageFlipAudioClip, this.gameObject.transform, 1f);

		this.gameObject.transform.GetChild(m_currentPageIndex).GetComponent<PageFlipper>().FlipForward();
		m_currentPageIndex++;

		m_currentPage = (PageID)m_currentPageIndex;

		OnPageFlip?.Invoke(m_currentPage);

		if (m_currentPage == PageID.Chapter1Introduction)
		{
			SceneManager.LoadScene("Chapter1GameplayScene", LoadSceneMode.Additive);
		}	

		if (m_currentPage == PageID.Chapter1Level1Gameplay)
		{
			ManagersManager.Get<MusicManager>().Play(MusicContext.Chapter1);
		}

		if (m_currentPage == PageID.Chapter2Introduction)
		{
			ManagersManager.Get<PlayerManager>().DestroyPlayer();
			SceneManager.LoadScene("Chapter2GameplayScene", LoadSceneMode.Additive);
		}

		if (m_currentPage == PageID.Chapter2Level1Gameplay)
		{
			SceneManager.UnloadSceneAsync("Chapter1GameplayScene");
			ManagersManager.Get<MusicManager>().Play(MusicContext.Chapter2);
		}

		if (m_currentPage == PageID.ThanksForPlayingThePrototype)
		{
			SceneManager.UnloadSceneAsync("Chapter2GameplayScene");

			ManagersManager.Get<PlayerManager>().DestroyPlayer();
			ManagersManager.Get<MusicManager>().Play(MusicContext.RegularPages);
		}
    }
    
    public void FlipPageBackward()
    {
	    SFXManager.Instance.PlaySFXClip(m_bookSettings.PageFlipAudioClip, this.gameObject.transform, 1f);

	    this.gameObject.transform.GetChild(m_currentPageIndex-1).GetComponent<PageFlipper>().FlipBackward();
	    m_currentPageIndex--;

    	m_currentPage = (PageID)m_currentPageIndex;

		OnPageFlip?.Invoke(m_currentPage);
    }

	public void FlipToPage(int index)
	{
		StartCoroutine(MoveBookToRight());
		
		StartCoroutine(FlipToPageRoutine(index));
	}

	private IEnumerator FlipToPageRoutine(int index)
	{
		int i = 0;
		
		while (i < index)
		{
			FlipPage();
			i++;
			yield return new WaitForSeconds(0.2f);
		}
	}

	private void ApplyPageTextures(Transform child, Material front, Material back)
	{
		MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
		Material[] materials = meshRenderer.materials;
		materials[1] = back;
		materials[2] = front;
		meshRenderer.materials = materials;
	}

	private void AddPageClickDetector(Transform child, string targetCanvasName)
	{
		PageClickDetector pageClickDetector = child.gameObject.AddComponent<PageClickDetector>();
		pageClickDetector.SetTargetCanvas(targetCanvasName);
	}

	public void CloseBookInstantly()
{
    Debug.Log("Close Book Instantly called");
    int totalPages = transform.childCount;

    for (int i = m_currentPageIndex; i < totalPages; i++)
    {
        PageFlipper flipper = transform.GetChild(i).GetComponent<PageFlipper>();
        flipper.FlipForwardInstant();
    }

    m_currentPageIndex = totalPages;
    m_currentPage = PageID.CoverBack;

    // Wir rufen die Bewegung mit dem Wissen auf, dass wir am Ende sind
    StartCoroutine(MoveBookToCenter(true)); 
}

	private IEnumerator SnapBookClosed()
	{
    	float t = 0f;
    	Quaternion start = transform.rotation;
    	Quaternion end = Quaternion.Euler(0, 180, 0);

    	while (t < 1f)
    	{
        	t += Time.deltaTime * 3f;
        	transform.rotation = Quaternion.Slerp(start, end, t);
        	yield return null;
    	}
	}







	private IEnumerator DebugStartRoutine()
	{
    	// Nutze localPosition statt position, um Konflikte mit Parent-Objekten zu vermeiden
    	transform.localPosition = new Vector3(2.5f, transform.localPosition.y, transform.localPosition.z); 
    
    	int targetIndex = (int)m_debugStartPage;

    	while (m_currentPageIndex < targetIndex)
    	{
        	FlipPageInstantly(); 

        	if (m_currentPage == PageID.Chapter1Introduction || m_currentPage == PageID.Chapter2Introduction)
        	{
            	yield return new WaitForSeconds(0.1f);
        	}
		}
    }

	// Eine Kopie deiner FlipPage, aber ohne Sound und ohne Zeitverzögerung
	public void FlipPageInstantly()
	{
    	// Seite grafisch sofort umlegen
    	this.gameObject.transform.GetChild(m_currentPageIndex).GetComponent<PageFlipper>().FlipForwardInstant();
    
    	m_currentPageIndex++;
    	m_currentPage = (PageID)m_currentPageIndex;

    	// WICHTIG: Die Events müssen feuern, damit Kameras und Player initialisiert werden!
    	OnPageFlip?.Invoke(m_currentPage);

    	// Hier die Logik-Checks kopieren (oder in eine eigene Methode auslagern)
    	if (m_currentPage == PageID.Chapter1Introduction)
        	SceneManager.LoadScene("Chapter1GameplayScene", LoadSceneMode.Additive);
    
    	if (m_currentPage == PageID.Chapter1Level1Gameplay)
        	ManagersManager.Get<MusicManager>().Play(MusicContext.Chapter1);

    	if (m_currentPage == PageID.Chapter2Introduction)
        	SceneManager.LoadScene("Chapter2GameplayScene", LoadSceneMode.Additive);

    	if (m_currentPage == PageID.Chapter2Level1Gameplay)
        	ManagersManager.Get<MusicManager>().Play(MusicContext.Chapter2);
	}
}
