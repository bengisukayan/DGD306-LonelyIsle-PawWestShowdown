using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartScreen : MonoBehaviour
{
    public Animator startAnimator;
    public VideoPlayer videoPlayer;
    public Canvas startCanvas;


    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartGame();
        }
    }

    void StartGame() {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence() {
        /*
        yield return new WaitForSeconds(2f); // video length

        startCanvas.enabled = false;
        videoPlayer.Play();

        // Wait until the video finishes
        while (videoPlayer.isPlaying)
            yield return null;
        */
        yield return new WaitForSeconds(1f); // delete later
        SceneManager.LoadScene("Tutorial");
    }
}
