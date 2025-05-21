using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float flashSpeed = 2f;

    void Update()
    {
        float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);

        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
