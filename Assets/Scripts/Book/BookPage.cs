using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BookPage : MonoBehaviour
{
    [SerializeField] private Camera m_pageCamera;
    [SerializeField] private BookUIConfig m_bookUIConfig;
    [SerializeField] private BookLibraryConfig m_bookLibraryConfig;

    private BookPageConfig m_bookPageConfig;
    
    [SerializeField] private PageID ID;

    private void Awake()
    {
        m_bookPageConfig = m_bookLibraryConfig.BookPageConfigs.Find(c => c.ID == ID);

        InstantiateUICanvas(m_pageCamera);
    }
    
    private void InstantiateUICanvas(Camera targetCamera)
    {
        GameObject canvasInstance = Instantiate(m_bookUIConfig.PagesUICanvasPrefab, this.gameObject.transform);
        canvasInstance.name = $"{gameObject.name}UICanvas";
        
        Canvas canvas = canvasInstance.GetComponent<Canvas>();
        canvas.worldCamera = targetCamera;
        canvas.planeDistance = 10;

        canvasInstance.AddComponent<CanvasClickHandler>();

        int pageLayer = targetCamera.gameObject.layer;

        canvasInstance.layer = pageLayer;

        if (m_bookPageConfig.ShowPageBorder)
        {
            GameObject pageBorder = Instantiate(m_bookUIConfig.PageBorderPrefab, canvasInstance.transform);
            pageBorder.layer = pageLayer;
        }
        
        int pageIndex = m_bookLibraryConfig.BookPageConfigs.FindIndex(c => c.ID == ID);
        
        if (m_bookPageConfig.ShowNextPageButton)
        {
            GameObject newButton = Instantiate(m_bookUIConfig.NextPageButtonPrefab, canvasInstance.gameObject.transform);
            newButton.transform.localPosition = newButton.transform.localPosition + m_bookUIConfig.NextPageButtonOffset;
            newButton.layer = pageLayer;
        }
        
        if (m_bookPageConfig.ShowPreviousPageButton)
        {
            GameObject newButton = Instantiate(m_bookUIConfig.PreviousPageButtonPrefab, canvasInstance.gameObject.transform);
            newButton.transform.localPosition = newButton.transform.localPosition + m_bookUIConfig.PreviousPageButtonOffset;
            newButton.layer = pageLayer;
        }
        
        if (m_bookPageConfig.ShowPageNumberLeft)
        {
            GameObject pageNumber = Instantiate(m_bookUIConfig.PageNumberLeftPrefab, canvasInstance.gameObject.transform);
            pageNumber.transform.localPosition = pageNumber.transform.localPosition + m_bookUIConfig.PageNumberLeftOffset;

            pageNumber.GetComponent<TextMeshProUGUI>().text = (pageIndex * 2).ToString();
            pageNumber.layer = pageLayer;
        }
        
        if (m_bookPageConfig.ShowPageNumberRight)
        {
            GameObject pageNumber = Instantiate(m_bookUIConfig.PageNumberRightPrefab, canvasInstance.gameObject.transform);
            pageNumber.transform.localPosition = pageNumber.transform.localPosition + m_bookUIConfig.PageNumberRightOffset;
            
            pageNumber.GetComponent<TextMeshProUGUI>().text = (pageIndex * 2 + 1).ToString();
            pageNumber.layer = pageLayer;
        }
    }
}