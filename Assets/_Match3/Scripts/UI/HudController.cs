using UnityEngine;
using TMPro;

public class HudController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScore(0);
    }

    public void UpdateMoves(int moves)
    {
        movesText.text = moves.ToString();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
