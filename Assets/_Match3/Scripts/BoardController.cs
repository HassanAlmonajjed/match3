using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DG.Tweening;

public class BoardController : MonoBehaviour
{
    [SerializeField] private TileController tilePrefab;
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;

    [Header("Animations")]
    [SerializeField] private float swipeDuration = 0.2f; // Duration of swipe animation
    [SerializeField] private float collapseDuration = 0.3f; // Duration of collapse animation
    [SerializeField] private float collapseStaggerDelay = 0.02f; // Delay between tiles falling

    private Board board;
    private readonly Dictionary<Vector2Int, TileController> tileControllers = new();

    private const float gap = 0.1f; 
    private const int maxIterations = 20;

    void Start()
    {
        board = new Board(width, height);
        board.Populate();
        GenerateBoard();
        
        TileController.PlayerSwiped += OnPlayerSwiped;
    }

    private void OnDestroy()
    {
        TileController.PlayerSwiped -= OnPlayerSwiped;
    }

    public void GenerateBoard()
    {
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Tile tile = board.GetTileAtPosition(new Vector2Int(x, y));
                if (tile != null)       
                {
                    // Calculate position with gaps relative to board starting point
                    Vector3 position = transform.position + new Vector3(x * (1f + gap), -y * (1f + gap), 0);

                    // Instantiate tile prefab as child of BoardController
                    TileController tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);

                    tileControllers[new Vector2Int(x, y)] = tileObj;

                    tileObj.SetTileColor(tile.id);
                }
            }
        }
    }

    private void OnPlayerSwiped(TileController swipedTile, SwipeDirection direction)
    {
        // Find the position of the swiped tile
        Vector2Int swipedPos = Vector2Int.zero;
        bool found = false;

        foreach (var kvp in tileControllers)
        {
            if (kvp.Value == swipedTile)
            {
                swipedPos = kvp.Key;
                found = true;
                break;
            }
        }

        if (!found)
            return;

        // Get the neighbor position
        Vector2Int neighborPos = board.GetTileAtDirection(swipedPos, direction);

        // Check if neighbor position is valid
        if (neighborPos.x == -1 || neighborPos.y == -1)
            return;

        // Check if both tiles exist
        if (!tileControllers.ContainsKey(neighborPos))
            return;

        TileController neighborTile = tileControllers[neighborPos];

        // Swap in the board logic
        board.Swipe(swipedPos, neighborPos);

        // Animate the tiles
        TileController.SwipeTiles(swipedTile, neighborTile);

        // Swap tile controller references in dictionary
        tileControllers[swipedPos] = neighborTile;
        tileControllers[neighborPos] = swipedTile;

        // Check for matches after a brief delay to let animation finish
        StartCoroutine(CheckAndClearMatches());
    }

    private IEnumerator CheckAndClearMatches()
    {
        yield return new WaitForSeconds(swipeDuration);

        // Detect matches
        var matches = board.DetectMatch();

        if (matches.Count > 0)
        {
            Debug.Log($"Matches detected at {matches.Count} positions");
            
            // Clear matches in board and destroy tile objects
            ClearMatches(matches);
            board.Print();

            // Collapse the board model so tiles fall into their new positions
            board.CollapseColumns();

            // Animate existing TileController GameObjects to match the updated model positions
            var newTileControllers = new Dictionary<Vector2Int, TileController>();

            // Create a sequence for all move animations so we can wait for completion
            Sequence seq = DOTween.Sequence();

            for (int x = 0; x < board.Width; x++)
            {
                // Get controllers in this column ordered from top (small y) to bottom (large y)
                var controllersInColumn = tileControllers
                    .Where(kvp => kvp.Key.x == x)
                    .OrderBy(kvp => kvp.Key.y)
                    .Select(kvp => kvp.Value)
                    .ToList();

                int count = controllersInColumn.Count;
                for (int i = 0; i < count; i++)
                {
                    int newY = board.Height - count + i;
                    var controller = controllersInColumn[i];
                    Vector3 newPos = transform.position + new Vector3(x * (1f + gap), -newY * (1f + gap), 0);

                    // Insert tween with staggered delay
                    float delay = i * collapseStaggerDelay;
                    seq.Insert(delay, controller.transform.DOMove(newPos, collapseDuration).SetEase(Ease.InOutQuad));

                    // Remember mapping so we can update dictionary after animation
                    newTileControllers[new Vector2Int(x, newY)] = controller;
                }
            }

            // Wait for all move animations to finish
            if (seq.Duration(false) > 0)
                yield return seq.WaitForCompletion();

            // Replace the dictionary contents with the updated positions
            tileControllers.Clear();
            foreach (var kvp in newTileControllers)
                tileControllers[kvp.Key] = kvp.Value;

            // Refill the board model (creates new Tile instances for empty spots)
            board.Refill();

            // Instantiate TileController objects for any new tiles created during refill
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (!tileControllers.ContainsKey(pos))
                    {
                        Tile tile = board.GetTileAtPosition(pos);
                        if (tile != null)
                        {
                            Vector3 position = transform.position + new Vector3(x * (1f + gap), -y * (1f + gap), 0);
                            TileController tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                            tileObj.SetTileColor(tile.id);
                            tileControllers[pos] = tileObj;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void ClearMatches(HashSet<Vector2Int> matches)
    {
        // Clear matches in the board model
        board.ClearMatches(matches);

        // Destroy tile GameObjects for cleared positions
        foreach (var pos in matches)
        {
            if (tileControllers.ContainsKey(pos))
            {
                Destroy(tileControllers[pos].gameObject);
                tileControllers.Remove(pos);
            }
        }
    }

}
