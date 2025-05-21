using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public Animator startAnimator;
    public VideoPlayer videoPlayer;
    public Canvas startCanvas;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
    }

    void StartGame() {
        startAnimator.SetTrigger("PlayStart");
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence() {
        startAnimator.SetTrigger("PlayStart");

        yield return new WaitForSeconds(2f); // video length

        startCanvas.enabled = false;
        videoPlayer.Play();

        // Wait until the video finishes
        while (videoPlayer.isPlaying)
            yield return null;

        SceneManager.LoadScene("Tutorial");
    }
}
