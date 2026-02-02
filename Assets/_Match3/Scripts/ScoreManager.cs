using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public event Action<int> ScoreChanged;

    public const int ThreeMatchScore = 60;
    public const int FourMatchScore = 120;
    public const int FiveMatchScore = 200;

    private int[] scoreRequirement = new int[3]{500, 1500, 3000};

    private void Awake()
    {       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateScore(int matchCount)
    {
        int scoreToAdd = 0;

        if (matchCount >= 5)
        {
            scoreToAdd = FiveMatchScore;
        }
        else if (matchCount == 4)
        {
            scoreToAdd = FourMatchScore;
        }
        else if (matchCount == 3)
        {
            scoreToAdd = ThreeMatchScore;
        }

        Score += scoreToAdd;
        ScoreChanged?.Invoke(Score);
    }

    public int[] ScoreRequirement => scoreRequirement;
    public int MaxRequiredScore => scoreRequirement.Length > 0 ? scoreRequirement[scoreRequirement.Length - 1] : 0;

    public int CalculateStars(int score)
    {
        int stars = 0;
        for (int i = 0; i < scoreRequirement.Length; i++)
        {
            if (score >= scoreRequirement[i])
            {
                stars = i + 1;
            }
        }
        return stars;
    }
}
