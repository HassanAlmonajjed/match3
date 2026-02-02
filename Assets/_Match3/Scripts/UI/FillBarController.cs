using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBarController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private List<Image> stars;
    [SerializeField] private Sprite filledSprite;
    [SerializeField] private Sprite unfilledSprite;

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged += HandleScoreChanged;
            UpdateProgressByScore(ScoreManager.Instance.Score);
        }
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged -= HandleScoreChanged;
        }
    }

    private void HandleScoreChanged(int score)
    {
        UpdateProgressByScore(score);
    }

    private void UpdateProgressByScore(int score)
    {
        if (ScoreManager.Instance == null) return;

        int maxScore = ScoreManager.Instance.MaxRequiredScore;
        float progress = maxScore > 0 ? (float)score / maxScore : 0f;
        
        slider.value = progress;
        int filledCount = ScoreManager.Instance.CalculateStars(score);

        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].sprite = i < filledCount ? filledSprite : unfilledSprite;
        }
    }

    public void UpdateProgress(float progress)
    {
        slider.value = progress;
        // This method might be called from elsewhere, but we prefer UpdateProgressByScore
        // To maintain compatibility, we use the local calculation here if needed, 
        // but the main flow now uses the score-based one.
        int filledCount = CalculateFilledStars(progress);

        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].sprite = i < filledCount ? filledSprite : unfilledSprite;
        }
    }

    public int CalculateFilledStars(float progress)
    {
        return CalculateFilledStars(progress, stars.Count);
    }

    public static int CalculateFilledStars(float progress, int starCount)
    {
        int count = 0;
        for (int i = 0; i < starCount; i++)
        {
            float threshold = (float)(i + 1) / starCount * 0.9f;
            if (progress >= threshold)
            {
                count++;
            }
        }
        return count;
    }
}
