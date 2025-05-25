using UnityEngine;
using TMPro;

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
        scoreText.text = "Score: " + ScoreManager.Instance.CurrentScore.ToString();
        lives.text = "X" + FindObjectOfType<PlayerMovement>().lives.ToString();
    }
}
