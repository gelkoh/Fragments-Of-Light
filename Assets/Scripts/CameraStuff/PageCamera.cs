using UnityEngine;

public class PageCamera : MonoBehaviour
{
    [SerializeField] private PageID m_pageID;

    private void OnEnable()
    {
        ManagersManager.Get<CameraManager>().Register(m_pageID, GetComponent<Camera>());
    }

    private void OnDisable()
    {
        ManagersManager.Get<CameraManager>().Unregister(m_pageID, GetComponent<Camera>());
    }
}