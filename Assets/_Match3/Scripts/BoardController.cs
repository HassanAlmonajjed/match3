using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DG.Tweening;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance { get; private set; }

    [SerializeField] private TileController tilePrefab;
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;
    [SerializeField, Range(1, 7)] private int uniqueTiles = 5;
    [SerializeField] private float _gap = 0.1f;
    [SerializeField] private int maxIterations = 20;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private List<Vector2Int> excludedTiles = new();

    [Header("Sprites")]
    [SerializeField] private Sprite[] tileSprites;

    [Header("Animations")]
    [SerializeField] private float swipeDuration = 0.2f; // Duration of swipe animation
    [SerializeField] private float collapseDuration = 0.3f; // Duration of collapse animation
    [SerializeField] private float collapseStaggerDelay = 0.02f; // Delay between tiles falling

    private const float _tileSize = 1;
    private const float refillPoint = 3;
    public Board board;

    public Dictionary<Vector2Int, TileController> Tiles { get; private set; } = new();
    public bool IsBusy { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        board = new Board(width, height, uniqueTiles, excludedTiles);
        board.Populate();
        GenerateBoard();

        TileController.PlayerSwiped += OnPlayerSwiped;
        GameManager.Instance.SetState(GameState.Playing);
    }

    private void OnDestroy()
    {
        TileController.PlayerSwiped -= OnPlayerSwiped;
    }

    public void GenerateBoard()
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                Vector2Int tilePosition = new(i, j);

                Tile tile = board.GetTileAtPosition(tilePosition);
                if (tile == null)
                    continue;

                Vector3 position = CalculateTilePosition(i, j, _tileSize, _gap, _offset);

                TileController tileObj = Instantiate(tilePrefab, position, Quaternion.identity);

                Tiles.Add(tilePosition, tileObj);

                tileObj.SetTileImage(tile.id);
            }
        }
    }

    public Vector3 CalculateTilePosition(int i, int j, float tileSize, float gap, Vector3 offset)
    {
        float x = i * (tileSize + gap);
        float y = -j * (tileSize + gap);
        Vector3 position = offset + new Vector3(x, y);
        return position;
    }

    private void OnPlayerSwiped(TileController swipedTile, SwipeDirection direction)
    {
        if (IsBusy)
            return;

        if (GameManager.Instance.GameState != GameState.Playing)
            return;

        StartCoroutine(SwipeCoroutine(swipedTile, direction));
    }

    private IEnumerator SwipeCoroutine(TileController swipedTile, SwipeDirection direction)
    {
        IsBusy = true;

        Vector2Int swipedPos = GetTilePosition(swipedTile);

        if (swipedPos == new Vector2Int(-1, -1))
        {
            IsBusy = false;
            yield break;
        }

        // Get the neighbor position
        Vector2Int neighborPos = board.GetTileAtDirection(swipedPos, direction);

        // Check if neighbor position is valid
        if (neighborPos.x == -1 || neighborPos.y == -1 || !Tiles.ContainsKey(neighborPos))
        {
            IsBusy = false;
            yield break;
        }

        // Perform initial swap
        SwapTiles(swipedPos, neighborPos);

        // Wait for swipe animation
        yield return new WaitForSeconds(swipeDuration);

        // Check for matches
        var matches = board.DetectMatch();

        if (matches != null && matches.Count > 0)
        {
            // Process matches
            yield return StartCoroutine(ProcessMatches(matches));
        }
        else
        {
            // Revert swap
            SwapTiles(swipedPos, neighborPos);

            GameManager.Instance.DetectGameOver(); 

            // Wait for revert animation
            yield return new WaitForSeconds(swipeDuration);
        }

        IsBusy = false;
    }

    private Vector2Int GetTilePosition(TileController tile)
    {
        var entry = Tiles.FirstOrDefault(kvp => kvp.Value == tile);
        if (entry.Value != null)
        {
            return entry.Key;
        }
        return new Vector2Int(-1, -1);
    }

    private void SwapTiles(Vector2Int pos1, Vector2Int pos2)
    {
        TileController tile1 = Tiles[pos1];
        TileController tile2 = Tiles[pos2];

        board.Swipe(pos1, pos2);
        TileController.SwipeTiles(tile1, tile2, swipeDuration);

        Tiles[pos1] = tile2;
        Tiles[pos2] = tile1;
    }

    private IEnumerator ProcessMatches(HashSet<Vector2Int> initialMatches)
    {
        HashSet<Vector2Int> matches = initialMatches;
        int iterations = 0;

        while (matches != null && matches.Count > 0)
        {
            ScoreManager.Instance.UpdateScore(matches.Count);
            ClearMatches(matches);

            yield return StartCoroutine(CollapseAndAnimate());
            yield return StartCoroutine(RefillAndAnimate());

            iterations++;
            if (iterations >= maxIterations)
            {
                Debug.LogWarning($"Reached max iterations ({maxIterations}) while resolving matches.");
                break;
            }

            matches = board.DetectMatch();
        }

        GameManager.Instance.DetectGameOver();
    }

    private IEnumerator CollapseAndAnimate()
    {
        board.CollapseColumns();

        var newTiles = Collapse();
        Sequence seq = AnimateCollapse(newTiles);

        if (seq.Duration(false) > 0)
            yield return seq.WaitForCompletion();

        Tiles.Clear();
        foreach (var tile in newTiles)
            Tiles[tile.Key] = tile.Value;
    }

    public Dictionary<Vector2Int, TileController> Collapse()
    {
        Dictionary<Vector2Int, TileController> newTileControllers = new();

        for (int i = 0; i < board.Width; i++)
        {
            var existingControllers = Tiles
                .Where(kvp => kvp.Key.x == i)
                .OrderBy(kvp => kvp.Key.y)
                .Select(kvp => kvp.Value)
                .ToList();

            int controllerIndex = 0;

            for (int j = 0; j < board.Height; j++)
            {
                var pos = new Vector2Int(i, j);
                var tile = board.GetTileAtPosition(pos);

                if (tile == null)
                    continue;

                if (controllerIndex < existingControllers.Count)
                {
                    var controller = existingControllers[controllerIndex];
                    newTileControllers[pos] = controller;
                    controllerIndex++;
                }
                else
                {
                    Debug.LogError(
                        $"Collapse Mismatch: Board has more tiles than Controller at Column {i}. Board needs tile at {j}, but we only had {existingControllers.Count} controllers."
                    );
                }
            }

            if (controllerIndex < existingControllers.Count)
            {
                Debug.LogWarning(
                    $"Collapse Mismatch: Controller has MORE tiles than Board at Column {i}. Used {controllerIndex} of {existingControllers.Count} controllers."
                );
            }
        }

        return newTileControllers;
    }

    private Sequence AnimateCollapse(Dictionary<Vector2Int, TileController> tiles)
    {
        Sequence seq = DOTween.Sequence();

        foreach (var kvp in tiles)
        {
            Vector2Int gridPos = kvp.Key;
            TileController controller = kvp.Value;

            if (controller == null) continue;

            Vector3 targetPos = CalculateTilePosition(gridPos.x, gridPos.y, _tileSize, _gap, _offset);

            // Optional safety: skip if already in place
            if (controller.transform.position == targetPos)
                continue;

            seq.Join(
                controller.transform
                    .DOMove(targetPos, collapseDuration)
                    .SetEase(Ease.InOutQuad)
                    .SetLink(controller.gameObject)
            );
        }

        return seq;
    }


    private IEnumerator RefillAndAnimate()
    {
        board.Refill();

        var addedTiles = Refill();

        Sequence fillSeq = AnimateRefill(addedTiles);

        if (fillSeq.Duration(false) > 0)
            yield return fillSeq.WaitForCompletion();
    }

    public Dictionary<Vector2Int, TileController> Refill()
    {
        Dictionary<Vector2Int, TileController> addedTiles = new();

        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                var pos = new Vector2Int(i, j);
                if (Tiles.ContainsKey(pos))
                    continue;

                Tile tile = board.GetTileAtPosition(pos);
                if (tile == null)
                    continue;

                Vector3 finalPos = CalculateTilePosition(i, j, _tileSize, _gap, _offset);
                Vector3 spawnPos = finalPos;
                spawnPos.y = refillPoint;

                TileController tileObj = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
                tileObj.SetTileImage(tile.id);

                addedTiles[pos] = tileObj;
                Tiles[pos] = tileObj;
            }
        }

        return addedTiles;
    }

    private Sequence AnimateRefill(Dictionary<Vector2Int, TileController> addedTiles)
    {
        Sequence fillSeq = DOTween.Sequence();
        float fillDelayCounter = 0f;

        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                var pos = new Vector2Int(i, j);
                if (addedTiles.TryGetValue(pos, out var tileObj))
                {
                    if (tileObj == null) continue;
                    Vector3 finalPos = CalculateTilePosition(i, j, _tileSize, _gap, _offset);
                    fillSeq.Insert(fillDelayCounter, tileObj.transform.DOMove(finalPos, collapseDuration)
                        .SetEase(Ease.OutBounce)
                        .SetLink(tileObj.gameObject));
                    fillDelayCounter += collapseStaggerDelay;
                }
            }
        }

        return fillSeq;
    }

    public void ClearMatches(HashSet<Vector2Int> matches)
    {
        board.ClearMatches(matches);

        foreach (var match in matches)
        {
            if (Tiles.TryGetValue(match, out var controller))
            {
                if (controller != null)
                {
                    var tileObj = controller.gameObject;

                    // Kill any active tweens on the object before destroying it
                    tileObj.transform.DOKill();

                    if (Application.isPlaying)
                    {
                        Destroy(tileObj);
                    }
                    else
                    {
                        DestroyImmediate(tileObj);
                    }
                }
                Tiles.Remove(match);
            }
        }
    }

    public Sprite GetSpriteForTileById(int id)
    {
        int index = id - 1;
        if (tileSprites == null || index < 0 || index >= tileSprites.Length)
        {
            Debug.LogWarning($"Sprite for tile ID {id} not found.");
            return null;
        }
        return tileSprites[index];
    }
}
