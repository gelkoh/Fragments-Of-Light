using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class Book : MonoBehaviour
{
	private bool m_useDebugStart = false;
	private PageID m_debugStartPage = PageID.Chapter2Level4Gameplay;

	private Material m_originalRightMaterial;
	private Material m_originalLeftMaterial;
	private string m_originalTargetCanvas;



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
		GameStateManager.OnEnd += HandleEndGame;
    }

    void OnDisable()
    {
		GameStateManager.OnStart -= HandleStartGame;
		GameStateManager.OnEnd -= HandleEndGame;
    }
    
    void Start()
    {
		int counter = 0;		

		foreach (Transform child in this.gameObject.transform)
		{
			child.gameObject.AddComponent<PageFlipper>();
			child.gameObject.AddComponent<MeshCollider>();

			MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
			Material[] materials = meshRenderer.materials;

			if (materials.Length == 3)
			{
				materials[0] = m_bookSettings.UnusedPageMaterial;

				if (counter > 12 && counter < transform.childCount - 2) {
					materials[1] = m_bookSettings.UnusedPageMaterial;
                    materials[2] = m_bookSettings.UnusedPageMaterial;
                }
			} 

			meshRenderer.materials = materials;

			if (counter == 0)
			{
				ApplyPageTextures(child, m_bookSettings.CoverFrontMaterial, m_bookSettings.Endpaper1LeftMaterial, m_bookSettings.CoverSideMaterial);
				AddPageClickDetector(child, "CoverUICanvas");
			}

			if (counter == 1)
			{
				ApplyPageTextures(child, m_bookSettings.Endpaper1RightMaterial, m_bookSettings.FrontispieceMaterial, m_bookSettings.UnusedPageMaterial);
				AddPageClickDetector(child, "Endpaper1UICanvas");
			}

			if (counter == 2)
			{
				ApplyPageTextures(child, m_bookSettings.TitlepageMaterial, m_bookSettings.Chapter1IntroductionLeftMaterial, m_bookSettings.UnusedPageMaterial);
				AddPageClickDetector(child, "FrontispieceAndTitlepageUICanvas");
			}

            if (counter == 3)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter1IntroductionRightMaterial, m_bookSettings.Chapter1Level1LeftMaterial, m_bookSettings.UnusedPageMaterial);
				AddPageClickDetector(child, "Chapter1IntroductionUICanvas");
			}

			if (counter == 4)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter1Level1RightMaterial, m_bookSettings.Chapter1Level2LeftMaterial, m_bookSettings.UnusedPageMaterial);
			}

			if (counter == 5)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter1Level2RightMaterial, m_bookSettings.Chapter2IntroductionLeftMaterial, m_bookSettings.UnusedPageMaterial);
			}

			if (counter == 6)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2IntroductionRightMaterial, m_bookSettings.Chapter2Level1LeftMaterial, m_bookSettings.UnusedPageMaterial);
				AddPageClickDetector(child, "Chapter2IntroductionUICanvas");
			}

			if (counter == 7)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2Level1RightMaterial, m_bookSettings.Chapter2Level2LeftMaterial, m_bookSettings.UnusedPageMaterial);
			}

			if (counter == 8)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2Level2RightMaterial, m_bookSettings.Chapter2Level3LeftMaterial, m_bookSettings.UnusedPageMaterial);
			}

			if (counter == 9)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2Level3RightMaterial, m_bookSettings.Chapter2Level4LeftMaterial, m_bookSettings.UnusedPageMaterial);
			}

			if (counter == 10)
			{
				ApplyPageTextures(child, m_bookSettings.Chapter2Level4RightMaterial, m_bookSettings.ThanksForPlayingThePrototypeLeftMaterial, m_bookSettings.UnusedPageMaterial);
			}

			if (counter == 11)
			{
				ApplyPageTextures(child, m_bookSettings.ThanksForPlayingThePrototypeRightMaterial, m_bookSettings.Endpaper2LeftMaterial, m_bookSettings.UnusedPageMaterial);
				AddPageClickDetector(child, "ThanksForPlayingThePrototypeUICanvas");
			}

			if (counter == 12)
			{
				ApplyPageTextures(child, m_bookSettings.Endpaper2RightMaterial, m_bookSettings.CoverBackMaterial, m_bookSettings.UnusedPageMaterial);
				AddPageClickDetector(child, "Endpaper2UICanvas");
			}

			if (counter == transform.childCount - 2)
			{
				ApplyPageTextures(child, m_bookSettings.Endpaper2RightMaterial, m_bookSettings.CoverBackMaterial, m_bookSettings.CoverSideMaterial);
			}

			if (counter == transform.childCount - 1)
			{
				ApplyPageTextures(child, m_bookSettings.SpineMaterial, m_bookSettings.SpineMaterial, m_bookSettings.CoverSideMaterial);
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

		Transform spine = transform.GetChild(transform.childCount - 1);
		spine.gameObject.SetActive(false);
	}

	private void HandleEndGame()
	{
		Transform spine = transform.GetChild(transform.childCount - 1);
		spine.gameObject.SetActive(true);
		m_currentPageIndex = 0;
		m_currentPage = PageID.CoverFront;
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
			ManagersManager.Get<PlayerManager>().DestroyPlayer();
			ManagersManager.Get<MusicManager>().Play(MusicContext.RegularPages);
		}

		if (m_currentPage == PageID.Endpaper2)
		{
			SceneManager.UnloadSceneAsync("Chapter2GameplayScene");
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

	private void ApplyPageTextures(Transform child, Material front, Material back, Material side)
	{
		MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
		Material[] materials = meshRenderer.materials;

		if (materials.Length == 3)
		{
			materials[0] = side;
			materials[1] = back;
			materials[2] = front;
		} 

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

	transform.GetChild(0).GetComponent<PageFlipper>().FlipForwardInstantWithCoverFix();
	transform.GetChild(totalPages - 2).GetComponent<PageFlipper>().FlipForwardInstantWithCoverFix();


    for (int i = m_currentPageIndex; i < totalPages; i++)
    {
        PageFlipper flipper = transform.GetChild(i).GetComponent<PageFlipper>();

         flipper.FlipForwardInstant();
    }

    m_currentPageIndex = 0;
    m_currentPage = PageID.CoverBack;

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


















	public void ShowMenuOnCurrentPageLeft(Material menuMaterial)
{
    Transform currentPage = transform.GetChild(m_currentPageIndex - 1);
    MeshRenderer meshRenderer = currentPage.GetComponent<MeshRenderer>();
    Material[] materials = meshRenderer.materials;

    m_originalRightMaterial = materials[1];
    materials[1] = menuMaterial;
    meshRenderer.materials = materials;

    PageClickDetector detector = currentPage.GetComponent<PageClickDetector>();
    if (detector != null)
    {
        detector.SetTargetCanvas("IngameMenuCanvas");
    }
    
    // WICHTIG: Setze Flipping für linke Seite
    GameObject menuCanvas = GameObject.Find("IngameMenuCanvas");
    if (menuCanvas != null)
    {
        CanvasClickHandler handler = menuCanvas.GetComponent<CanvasClickHandler>();
        if (handler != null)
        {
            // Linke Seite: Meist horizontal geflippt
            handler.SetFlipping(true, false);
        }
    }
}

public void ShowMenuOnCurrentPageRight(Material menuMaterial)
{
    Transform currentPage = transform.GetChild(m_currentPageIndex);
    MeshRenderer meshRenderer = currentPage.GetComponent<MeshRenderer>();
    Material[] materials = meshRenderer.materials;

    m_originalLeftMaterial = materials[2];
    materials[2] = menuMaterial;
    meshRenderer.materials = materials;

    PageClickDetector detector = currentPage.GetComponent<PageClickDetector>();
    if (detector != null)
    {
        m_originalTargetCanvas = detector.GetTargetCanvas();
        detector.SetTargetCanvas("IngameMenuCanvas");
    }
    
    // WICHTIG: Setze Flipping für rechte Seite
    GameObject menuCanvas = GameObject.Find("IngameMenuCanvas");
    if (menuCanvas != null)
    {
        CanvasClickHandler handler = menuCanvas.GetComponent<CanvasClickHandler>();
        if (handler != null)
        {
            // Rechte Seite: Normalerweise kein Flipping nötig
            handler.SetFlipping(false, false);
        }
    }
}

    public void HideMenuOnCurrentPageLeft()
    {
        Transform currentPage = transform.GetChild(m_currentPageIndex - 1);
        MeshRenderer meshRenderer = currentPage.GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.materials;

        // Original-Material zurücksetzen
        materials[1] = m_originalRightMaterial;
        meshRenderer.materials = materials;

        PageClickDetector detector = currentPage.GetComponent<PageClickDetector>();

		if (detector != null)
		{
			detector.SetTargetCanvas(m_originalTargetCanvas);
		}
    }

    public void HideMenuOnCurrentPageRight()
    {
        Transform currentPage = transform.GetChild(m_currentPageIndex);
        MeshRenderer meshRenderer = currentPage.GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.materials;

        // Original-Material zurücksetzen
        materials[2] = m_originalLeftMaterial;
        meshRenderer.materials = materials;

 		PageClickDetector detector = currentPage.GetComponent<PageClickDetector>();

		if (detector != null)
		{
			detector.SetTargetCanvas(m_originalTargetCanvas);
		}
    }
}
