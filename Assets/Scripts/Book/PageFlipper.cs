using UnityEngine;
using System.Collections;

public class PageFlipper : MonoBehaviour
{
    private float flipDuration = 1f;
    private bool isFlipping = false;

private Quaternion initialRotation;
void Awake()
    {
        // Wir merken uns die Rotation, wie sie aus Blender/dem Import kommt.
        initialRotation = transform.localRotation;
    }

    public void FlipForward()
    {
        if (!isFlipping)
			StartCoroutine(FlipRoutine(0, 180f)); // Von 0 Grad Abweichung zu -180 Grad    
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
            
            // Wir nutzen eine sanfte Kurve für das Umblättern
            float easedTime = Mathf.SmoothStep(0, 1, normalized);
            float currentAngle = Mathf.Lerp(startAngle, endAngle, easedTime);

            // DIE BERECHNUNG:
            // Wir nehmen die Import-Rotation und fügen eine Rotation UM die lokale X-Achse hinzu.
            // Vector3.right entspricht der lokalen roten Achse (X).
            transform.localRotation = initialRotation * Quaternion.AngleAxis(currentAngle, Vector3.forward);

            yield return null;
        }

        // Exakten Endwert setzen
        transform.localRotation = initialRotation * Quaternion.AngleAxis(endAngle, Vector3.forward);
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