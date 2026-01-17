using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections.Generic;

public class CanvasClickHandler : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);

    void Awake()
    {
        canvas = this.gameObject.GetComponent<Canvas>();
    }

    public void HandlePageClick(Vector2 uvHit)
    {
        Debug.Log("UV Hit: " + uvHit);
        if (canvas == null) return;
        
        float pixelX = uvHit.x * referenceResolution.x;
        float pixelY = uvHit.y * referenceResolution.y;

        Debug.Log(pixelX + ", " + pixelY);
        
        SimulateUIClick(new Vector2(pixelX, pixelY));
    }

    private void SimulateUIClick(Vector2 pixelPosition)
    {
        if (EventSystem.current == null) return;

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = pixelPosition; 

        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null) return;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            GameObject clickedObject = results[0].gameObject;
            
            Debug.Log("Clicked object: ", clickedObject);
            
            UnityEngine.UI.Slider slider = clickedObject.GetComponentInParent<UnityEngine.UI.Slider>();
            
            if (slider != null)
            {
                ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerDownHandler);
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    slider.handleRect.parent as RectTransform, 
                    pixelPosition, 
                    canvas.worldCamera, 
                    out Vector2 localPointerPosition
                );
                
                ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerClickHandler);
                
                return; 
            }
            
            ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }
}