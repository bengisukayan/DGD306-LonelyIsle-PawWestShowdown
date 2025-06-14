using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private static bool scoreWasReset = false;

    private const int maxHighScores = 7;
    private const string scoreKey = "HighScore_";


    public int CurrentScore { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (!scoreWasReset)
        {
            Instance.ResetScore();
            scoreWasReset = true;


            if (player != null)
            {
                player.health = 100;
                player.lives = 9;
            }
        }
        else
        {
            if (player != null)
            {
                Debug.Log("Player found, setting initial health and lives.");
                player.health = 100;
            }
        }
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
    }

    public void ResetScore()
    {
        CurrentScore = 0;
    }

    public void TrySaveScore(string playerName)
    {
        List<(string name, int score)> scores = LoadScores();
        scores.Add((playerName, CurrentScore));
        scores.Sort((a, b) => b.score.CompareTo(a.score)); // High to low
        if (scores.Count > maxHighScores)
            scores.RemoveAt(scores.Count - 1);

        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetString(scoreKey + "Name" + i, scores[i].name);
            PlayerPrefs.SetInt(scoreKey + "Value" + i, scores[i].score);
        }
        PlayerPrefs.Save();
    }

    public List<(string name, int score)> LoadScores()
    {
        List<(string name, int score)> scores = new();
        for (int i = 0; i < maxHighScores; i++)
        {
            string name = PlayerPrefs.GetString(scoreKey + "Name" + i, "----");
            int score = PlayerPrefs.GetInt(scoreKey + "Value" + i, 0);
            scores.Add((name, score));
        }
        return scores;
    }
}
