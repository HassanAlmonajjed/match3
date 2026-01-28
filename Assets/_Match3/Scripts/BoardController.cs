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
    private readonly Dictionary<Vector2Int, TileController> tiles = new();

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

                    tiles[new Vector2Int(x, y)] = tileObj;

                    tileObj.SetTileColor(tile.id);
                }
            }
        }
    }

    private void OnPlayerSwiped(TileController swipedTile, SwipeDirection direction)
    {
        Vector2Int swipedPos = Vector2Int.zero;
        bool found = false;

        var swipedEntry = tiles.FirstOrDefault(kvp => kvp.Value == swipedTile);
        if (swipedEntry.Value != null)
        {
            swipedPos = swipedEntry.Key;
            found = true;
        }

        if (!found)
            return;

        // Get the neighbor position
        Vector2Int neighborPos = board.GetTileAtDirection(swipedPos, direction);

        // Check if neighbor position is valid
        if (neighborPos.x == -1 || neighborPos.y == -1)
            return;

        // Check if both tiles exist
        if (!tiles.ContainsKey(neighborPos))
            return;

        TileController neighborTile = tiles[neighborPos];

        // Swap in the board logic
        board.Swipe(swipedPos, neighborPos);

        // Animate the tiles
        TileController.SwipeTiles(swipedTile, neighborTile);

        // Swap tile controller references in dictionary
        tiles[swipedPos] = neighborTile;
        tiles[neighborPos] = swipedTile;

        // Check for matches after a brief delay to let animation finish
        StartCoroutine(CheckAndClearMatches());
    }

    private IEnumerator CheckAndClearMatches()
    {
        // Wait for swipe animation to finish before checking for matches
        yield return new WaitForSeconds(swipeDuration);

        int iterations = 0;

        while (true)
        {
            var matches = board.DetectMatch();

            if (matches == null || matches.Count <= 0)
                break;

            ClearMatches(matches);

            yield return StartCoroutine(CollapseAndAnimate());

            yield return StartCoroutine(RefillAndAnimate());

            iterations++;
            if (iterations >= maxIterations)
            {
                Debug.LogWarning($"Reached max iterations ({maxIterations}) while resolving matches.");
                break;
            }
        }

        board.Print();
    }

    private IEnumerator CollapseAndAnimate()
    {
        board.CollapseColumns();

        var newTiles = new Dictionary<Vector2Int, TileController>();
        Sequence seq = DOTween.Sequence();
        Collapse(newTiles, seq);

        if (seq.Duration(false) > 0)
            yield return seq.WaitForCompletion();

        tiles.Clear();
        foreach (var tile in newTiles)
            tiles[tile.Key] = tile.Value;
    }

    private void Collapse(Dictionary<Vector2Int, TileController> newTileControllers, Sequence seq)
    {
        for (int x = 0; x < board.Width; x++)
        {
            var tilesInColumn = tiles
                .Where(kvp => kvp.Key.x == x)
                .OrderBy(kvp => kvp.Key.y)
                .Select(kvp => kvp.Value)
                .ToList();

            int count = tilesInColumn.Count;
            for (int i = 0; i < count; i++)
            {
                int newY = board.Height - count + i;
                var controller = tilesInColumn[i];
                Vector3 newPos = transform.position + new Vector3(x * (1f + gap), -newY * (1f + gap), 0);

                float delay = i * collapseStaggerDelay;
                seq.Insert(delay, controller.transform.DOMove(newPos, collapseDuration).SetEase(Ease.InOutQuad));

                newTileControllers[new Vector2Int(x, newY)] = controller;
            }
        }
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
                if (tiles.ContainsKey(pos))
                    continue;

                Tile tile = board.GetTileAtPosition(pos);
                if (tile == null)
                    continue;

                Vector3 spawnPos = transform.position + new Vector3(x * (1f + gap), 1f * (1f + gap), 0);
                Vector3 finalPos = transform.position + new Vector3(x * (1f + gap), -y * (1f + gap), 0);

                TileController tileObj = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                tileObj.SetTileColor(tile.id);

                // Register immediately so lookups work
                tiles[pos] = tileObj;

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
            if (tiles.ContainsKey(pos))
            {
                Destroy(tiles[pos].gameObject);
                tiles.Remove(pos);
            }
        }
    }
}
