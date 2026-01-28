using UnityEngine;

public class Starter : MonoBehaviour
{
    public Vector2Int start, end;
    void Start()
    {
        Board board = new(5, 5);
        board.Populate();
        board.Print();
        DetectMatch(board);
        board.Swipe(start, end);
        DetectMatch(board);
        board.Print();
        board.CollapseColumns();
        board.Print();
        board.Refill();
        board.Print();
    }

    private static void DetectMatch(Board board)
    {
        var matches = board.DetectMatch();
        if (matches.Count > 0)
        {
            Debug.Log("Matches detected at:");
            foreach (var pos in matches)
            {
                Debug.Log(pos);
            }

            board.ClearMatches(matches);
        }
        else
        {
            Debug.Log("No matches detected.");
        }
    }
}
