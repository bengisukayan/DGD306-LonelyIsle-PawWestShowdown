using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void Update()
    {
        scoreText.text = "Score: " + ScoreManager.Instance.CurrentScore.ToString();
    }
}
