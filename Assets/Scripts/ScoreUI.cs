using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text lives;

    void Start()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Credits" ||
            SceneManager.GetActiveScene().name == "ScoreScreen")
            return;
        scoreText.text = "Score: " + ScoreManager.Instance.CurrentScore.ToString();
        lives.text = "X" + FindObjectOfType<PlayerMovement>().lives.ToString();
    }
}
