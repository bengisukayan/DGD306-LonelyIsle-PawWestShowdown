using TMPro;
using UnityEngine;
using System.Collections;

public class NameInput : MonoBehaviour
{
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI scoreText;

    private readonly char[] userName = new char[4] { 'A', 'A', 'A', 'A' };
    private int currentLetter = 0;

    private float blinkInterval = 0.5f;
    private bool blinkOn = true;

    void Start()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + "300"; // Replace with actual score

        StartCoroutine(BlinkSelectedLetter());
        UpdateNameDisplay();
    }

    void Update()
    {
        bool changed = false;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLetter = (currentLetter + 1) % 4;
            changed = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLetter = (currentLetter - 1 + 4) % 4;
            changed = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            userName[currentLetter] = (char)(((userName[currentLetter] - 'A' + 25) % 26) + 'A');
            changed = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            userName[currentLetter] = (char)(((userName[currentLetter] - 'A' + 1) % 26) + 'A');
            changed = true;
        }

        if (changed)
        {
            UpdateNameDisplay();
        }
    }

    IEnumerator BlinkSelectedLetter()
    {
        while (true)
        {
            blinkOn = !blinkOn;
            UpdateNameDisplay();
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void UpdateNameDisplay()
    {
        string display = "";
        for (int i = 0; i < userName.Length; i++)
        {
            if (i == currentLetter && blinkOn)
            {
                display += $"<color=#00FFFF>{userName[i]}</color>";
            }
            else
            {
                display += userName[i];
            }
        }

        nameDisplay.text = display;
    }

    public void SubmitName()
    {
        ScoreManager.Instance.TrySaveScore(new string(userName));
    }
}
