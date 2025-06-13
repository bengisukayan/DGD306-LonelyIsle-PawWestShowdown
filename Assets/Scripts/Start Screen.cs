using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class StartScreen : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Canvas startCanvas;

    public Image logoImage;
    public Image bgImage;
    public TextMeshProUGUI pressStartText;

    public float fadeDuration = 1f;
    public float tintTransitionDuration = 1f;
    public AudioSource audioSource;


    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        audioSource.enabled = false;
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        float time = 0f;
        Color logoColor = logoImage.color;
        Color pressStartColor = pressStartText.color;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            logoImage.color = new Color(logoColor.r, logoColor.g, logoColor.b, alpha);
            pressStartText.color = new Color(pressStartColor.r, pressStartColor.g, pressStartColor.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        logoImage.color = new Color(logoColor.r, logoColor.g, logoColor.b, 0f);
        pressStartText.color = new Color(pressStartColor.r, pressStartColor.g, pressStartColor.b, 0f);

        time = 0f;
        Color startColor = bgImage.color;
        Color targetColor = Color.white;

        while (time < tintTransitionDuration)
        {
            bgImage.color = Color.Lerp(startColor, targetColor, time / tintTransitionDuration);
            time += Time.deltaTime;
            yield return null;
        }
        bgImage.color = targetColor;

        startCanvas.enabled = false;
        videoPlayer.Play();

        while (videoPlayer.frame < (long)videoPlayer.frameCount - 1)
            yield return null;

        SceneManager.LoadScene("CharacterSelect");
    }
}
