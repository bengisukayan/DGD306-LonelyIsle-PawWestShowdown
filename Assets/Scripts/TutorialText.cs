using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public string messageOnCollision = "AAAAAAAAAA";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        messageText.text = messageOnCollision;
    }
}
