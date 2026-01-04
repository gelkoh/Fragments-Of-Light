using UnityEngine;

public class MenuCameraBinder : MonoBehaviour
{
    private Canvas m_menuCanvas;

	private CameraManager m_cameraManager;
    
    private void Awake()
    {
        m_menuCanvas = this.gameObject.GetComponent<Canvas>();

		m_cameraManager = ManagersManager.Get<CameraManager>();
    }

    private void Start()
    {
        Book.OnPageFlip += HandlePageFlip;
    }
    
    private void OnDisable()
    {
        Book.OnPageFlip -= HandlePageFlip;
    }

    private void HandlePageFlip(PageID pageID)
    {
        Camera cam = m_cameraManager.GetCamera(pageID);

        if (cam != null)
        {
            m_menuCanvas.worldCamera = cam;
            m_menuCanvas.gameObject.layer = cam.gameObject.layer;
        }
    }
}
