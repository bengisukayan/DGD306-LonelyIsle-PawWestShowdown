using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class NameInput : MonoBehaviour
{
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI scoreText;

    private readonly char[] userName = new char[4] { 'A', 'A', 'A', 'A' };
    private int currentLetter = 0;

    private float blinkInterval = 0.5f;
    private bool blinkOn = true;

    private bool canNavigate = true;

    private void Start()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + ScoreManager.Instance.CurrentScore.ToString();

        StartCoroutine(BlinkSelectedLetter());
        UpdateNameDisplay();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed || !canNavigate) return;

        Vector2 input = context.ReadValue<Vector2>();
        bool changed = false;

        if (input.x > 0.5f)
        {
            currentLetter = (currentLetter + 1) % 4;
            changed = true;
        }
        else if (input.x < -0.5f)
        {
            currentLetter = (currentLetter - 1 + 4) % 4;
            changed = true;
        }
        else if (input.y > 0.5f)
        {
            userName[currentLetter] = (char)(((userName[currentLetter] - 'A' + 25) % 26) + 'A'); // Up
            changed = true;
        }
        else if (input.y < -0.5f)
        {
            userName[currentLetter] = (char)(((userName[currentLetter] - 'A' + 1) % 26) + 'A'); // Down
            changed = true;
        }

        if (changed)
        {
            UpdateNameDisplay();
            StartCoroutine(NavigationCooldown());
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        SubmitName();
    }

    private IEnumerator NavigationCooldown()
    {
        canNavigate = false;
        yield return new WaitForSeconds(0.2f);
        canNavigate = true;
    }

    private IEnumerator BlinkSelectedLetter()
    {
        while (true)
        {
            blinkOn = !blinkOn;
            UpdateNameDisplay();
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void UpdateNameDisplay()
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
        SceneManager.LoadScene("ScoreList");
    }
}
