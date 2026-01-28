using UnityEngine;
using System.Collections;

public class PageFlipper : MonoBehaviour
{
    private float flipDuration = 1f;
    private bool isFlipping = false;

    private Quaternion initialRotation;
    
    void Awake()
    {
        initialRotation = transform.localRotation;
    }

    public void FlipForward()
    {
        if (!isFlipping)
            StartCoroutine(FlipRoutine(0, 180f));
    }

    public void FlipBackward()
    {
        if (!isFlipping)
            StartCoroutine(FlipRoutine(180f, 0));
    }

    private IEnumerator FlipRoutine(float startAngle, float endAngle)
    {
        isFlipping = true;
        float t = 0;

        while (t < flipDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / flipDuration;
            
            float easedTime = Mathf.SmoothStep(0, 1, normalized);
            float currentAngle = Mathf.Lerp(startAngle, endAngle, easedTime);

            transform.localRotation = initialRotation * Quaternion.AngleAxis(currentAngle, Vector3.forward);

            yield return null;
        }

        transform.localRotation = initialRotation * Quaternion.AngleAxis(endAngle, Vector3.forward);
        isFlipping = false;
    }

    public void FlipForwardInstant()
    {
        Quaternion initialRot = transform.localRotation;
        Vector3 initialEuler = initialRot.eulerAngles;
        transform.localRotation = Quaternion.Euler(initialEuler.x, initialEuler.y, 180);
        enabled = false;
    }
    
    public void FlipForwardInstantWithCoverFix()
    {
        Quaternion initialRot = transform.localRotation;
        Vector3 initialEuler = initialRot.eulerAngles;
        transform.localRotation = Quaternion.Euler(initialEuler.x, initialEuler.y, 180);
        
        Vector3 pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, -0.01f, pos.z);
        
        enabled = false;
    }
}