using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public float scrollSpeed = 30f;
    public RectTransform creditsText;
    public float endYPosition = 1000f;


    private void Update()
    {
        if (creditsText.anchoredPosition.y < endYPosition)
        {
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
        else
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
