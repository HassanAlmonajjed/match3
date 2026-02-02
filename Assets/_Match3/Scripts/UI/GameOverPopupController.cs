using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameOverPopupController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private List<GameObject> stars;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button replayButton;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
        replayButton.onClick.AddListener(OnReplayButtonClicked);
        
        // Hide by default
        gameObject.SetActive(false);
    }

    public void SetupMenu(int score)
    {
        int starsCount = ScoreManager.Instance.CalculateStars(score);
        bool hasWin = starsCount >= 1;

        titleText.text = hasWin ? "Complete" : "Game Over";
        scoreText.text = score.ToString();

        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].SetActive(i < starsCount);
        }
        gameObject.SetActive(true);
    }

    private void OnNextButtonClicked()
    {
        Debug.Log("Next Button Clicked");
    }

    private void OnReplayButtonClicked()
    {
        Debug.Log("Replay Button Clicked");
    }
}
