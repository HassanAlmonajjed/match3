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

            // Collapse existing tiles and animate to their new positions
            yield return StartCoroutine(CollapseAndAnimate());

            // Refill model and animate new tiles falling in
            yield return StartCoroutine(RefillAndAnimate());
        }
    }

    private IEnumerator CollapseAndAnimate()
    {
        // Update model
        board.CollapseColumns();

        var newTileControllers = new Dictionary<Vector2Int, TileController>();
        Sequence seq = DOTween.Sequence();

        for (int x = 0; x < board.Width; x++)
        {
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

                float delay = i * collapseStaggerDelay;
                seq.Insert(delay, controller.transform.DOMove(newPos, collapseDuration).SetEase(Ease.InOutQuad));

                newTileControllers[new Vector2Int(x, newY)] = controller;
            }
        }

        if (seq.Duration(false) > 0)
            yield return seq.WaitForCompletion();

        // Update mapping
        tileControllers.Clear();
        foreach (var kvp in newTileControllers)
            tileControllers[kvp.Key] = kvp.Value;
    }

    private IEnumerator RefillAndAnimate()
    {
        // Refill model
        board.Refill();

        Sequence fillSeq = DOTween.Sequence();
        float fillDelayCounter = 0f;

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                var pos = new Vector2Int(x, y);
                if (tileControllers.ContainsKey(pos))
                    continue;

                Tile tile = board.GetTileAtPosition(pos);
                if (tile == null)
                    continue;

                Vector3 spawnPos = transform.position + new Vector3(x * (1f + gap), 1f * (1f + gap), 0);
                Vector3 finalPos = transform.position + new Vector3(x * (1f + gap), -y * (1f + gap), 0);

                TileController tileObj = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                tileObj.SetTileColor(tile.id);

                // Register immediately so lookups work
                tileControllers[pos] = tileObj;

                // Animate into place with staggered delay
                fillSeq.Insert(fillDelayCounter, tileObj.transform.DOMove(finalPos, collapseDuration).SetEase(Ease.OutBounce));
                fillDelayCounter += collapseStaggerDelay;
            }
        }

        if (fillSeq.Duration(false) > 0)
            yield return fillSeq.WaitForCompletion();
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
