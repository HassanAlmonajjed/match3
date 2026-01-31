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

    private int _currentScore;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MovesChanged += UpdateMoves;
            GameManager.Instance.GameStateChanged += OnGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MovesChanged -= UpdateMoves;
            GameManager.Instance.GameStateChanged -= OnGameStateChanged;
        }
    }

    private void Start()
    {
        UpdateScore(0);
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
        _currentScore = score;
        scoreText.text = score.ToString();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            // For now, we don't have a way to determine win/loss or stars from GameManager
            // So we'll pass defaults. In a real scenario, GameManager would provide this info.
            bool hasWin = false; 
            int stars = 0;
            
            // If there's a FillBarController, we might get stars from there
            FillBarController fillBar = FindFirstObjectByType<FillBarController>();
            if (fillBar != null)
            {
                // This is a bit of a hack, but without a clear way to get stars:
                // We'd need to know the current progress.
                // Assuming FillBarController has the current progress or we can calculate it.
                // For now, let's keep it simple as requested.
            }
            gameOverPopup.SetupMenu(stars, hasWin, _currentScore);
        }
    }
}
