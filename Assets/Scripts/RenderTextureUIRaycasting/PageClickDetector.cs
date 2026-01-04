using UnityEngine;
using UnityEngine.InputSystem;

public class PageClickDetector : MonoBehaviour
{
    private Camera m_mainCamera;
    
    private string m_targetCanvasName; 

    void Awake()
    {
        m_mainCamera = Camera.main; 
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        RaycastHit hit;
        Ray ray = m_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                Vector2 uvHit = hit.textureCoord;
                
                GameObject handlerObject = GameObject.Find(m_targetCanvasName);
                Debug.Log(m_targetCanvasName);

                if (handlerObject != null)
                {
                    CanvasClickHandler handler = handlerObject.GetComponent<CanvasClickHandler>();
                    if (handler != null)
                    {
                        handler.HandlePageClick(uvHit);
                    }
                }
            }
        }
    }

    public void SetTargetCanvas(string canvasName)
    {
        m_targetCanvasName = canvasName;
    }
}