using UnityEngine;

public class PulsingText : MonoBehaviour
{
    public RectTransform textTransform;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.1f;

    Vector3 originalScale;

    void Start()
    {
        originalScale = textTransform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        textTransform.localScale = originalScale * scale;
    }
}
