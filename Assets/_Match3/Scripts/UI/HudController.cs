using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameOverPopupController gameOverPopup;
    [SerializeField] private PausePopupController pausePopup;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MovesChanged += UpdateMoves;
            GameManager.Instance.GameStateChanged += OnGameStateChanged;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged += UpdateScore;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MovesChanged -= UpdateMoves;
            GameManager.Instance.GameStateChanged -= OnGameStateChanged;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged -= UpdateScore;
        }
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            UpdateScore(ScoreManager.Instance.Score);
        }
        else
        {
            UpdateScore(0);
        }

        if (GameManager.Instance != null)
        {
            UpdateMoves(GameManager.Instance.Moves);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
        }
    }

    private void OnPauseClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameState.Paused);
        }

        if (pausePopup != null)
        {
            pausePopup.gameObject.SetActive(true);
        }
    }

    public void UpdateMoves(int moves)
    {
        movesText.text = moves.ToString();
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            int finalScore = ScoreManager.Instance.Score;
            
            if (gameOverPopup != null)
            {
                gameOverPopup.SetupMenu(finalScore);
            }
        }
    }
}
