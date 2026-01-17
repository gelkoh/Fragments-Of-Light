using UnityEngine;
using System.Collections;

public class PageFlipper : MonoBehaviour
{
    private float flipDuration = 1f;
    private bool isFlipping = false;

    public void FlipForward()
    {
        if (!isFlipping)
            StartCoroutine(FlipPage(0, 180));
    }

    public void FlipBackward()
    {
        if (!isFlipping)
            StartCoroutine(FlipPage(180, 0));
    }

    private IEnumerator FlipPage(float startAngle, float endAngle)
    {
        isFlipping = true;
        float t = 0;
    
        Quaternion initialRotation = transform.localRotation;
        Vector3 initialEuler = initialRotation.eulerAngles;

        while (t < flipDuration)
        {
			t += Time.unscaledDeltaTime;            
			float normalized = t / flipDuration;
            float angle = Mathf.Lerp(startAngle, endAngle, normalized);
        
            transform.localRotation = Quaternion.Euler(initialEuler.x, initialEuler.y, angle);
        
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(initialEuler.x, initialEuler.y, endAngle);
        isFlipping = false;
    }

	public void FlipForwardInstant()
	{
        Quaternion initialRotation = transform.localRotation;
        Vector3 initialEuler = initialRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(initialEuler.x, initialEuler.y, 180);
    	enabled = false;
	}
}