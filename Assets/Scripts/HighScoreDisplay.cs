using UnityEngine;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    public TMP_Text highScoreText;

    void Start()
    {
        var scores = ScoreManager.Instance.LoadScores();
        highScoreText.text = "";

        foreach (var s in scores)
        {
            highScoreText.text += $"{s.name} - {s.score}\n";
        }
    }
}
