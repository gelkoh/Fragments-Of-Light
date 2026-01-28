using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CanvasClickHandler : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
    [SerializeField] private bool showDebugGizmos = true;
    
    private Vector2 lastClickPosition;

    void Awake()
    {
        canvas = this.gameObject.GetComponent<Canvas>();
    }

    public void HandlePageClick(Vector2 uvHit)
    {
        if (canvas == null) return;
        
        float pixelX = uvHit.x * referenceResolution.x;
        float pixelY = uvHit.y * referenceResolution.y;

        lastClickPosition = new Vector2(pixelX, pixelY);
        
        Debug.Log($"UV: {uvHit}, Pixel: {pixelX}, {pixelY}");
        
        SimulateUIClick(lastClickPosition);
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
            
            Debug.Log($"Clicked: {clickedObject.name} at {pixelPosition}");
            
            Slider slider = clickedObject.GetComponentInParent<Slider>();
            if (slider != null)
            {
                HandleSliderClick(slider, pointerData, pixelPosition);
                return;
            }
            
            ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
        else
        {
            Debug.Log("No UI element hit");
        }
    }
    
    private void HandleSliderClick(Slider slider, PointerEventData pointerData, Vector2 pixelPosition)
    {
        RectTransform sliderRect = slider.GetComponent<RectTransform>();
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            sliderRect,
            pixelPosition,
            canvas.worldCamera,
            out localPoint
        );
        
        Rect rect = sliderRect.rect;
        float normalizedValue;
        
        if (slider.direction == Slider.Direction.LeftToRight)
        {
            normalizedValue = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        }
        else if (slider.direction == Slider.Direction.RightToLeft)
        {
            normalizedValue = Mathf.InverseLerp(rect.xMax, rect.xMin, localPoint.x);
        }
        else if (slider.direction == Slider.Direction.BottomToTop)
        {
            normalizedValue = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);
        }
        else
        {
            normalizedValue = Mathf.InverseLerp(rect.yMax, rect.yMin, localPoint.y);
        }
        
        normalizedValue = Mathf.Clamp01(normalizedValue);
        float newValue = Mathf.Lerp(slider.minValue, slider.maxValue, normalizedValue);
        
        slider.value = newValue;
        
        Debug.Log($"Slider set to: {newValue} (normalized: {normalizedValue})");
    }
    
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || canvas == null) return;
        
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        Gizmos.color = Color.cyan;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        }
        
        if (lastClickPosition != Vector2.zero)
        {
            Vector2 viewportPos = new Vector2(
                lastClickPosition.x / referenceResolution.x,
                lastClickPosition.y / referenceResolution.y
            );
            
            Vector3 worldPos = Vector3.Lerp(
                Vector3.Lerp(corners[0], corners[1], viewportPos.x),
                Vector3.Lerp(corners[3], corners[2], viewportPos.x),
                viewportPos.y
            );
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(worldPos, 0.02f);
        }
    }
}