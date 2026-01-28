using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class BoardController : MonoBehaviour
{
    public float gap = 0.1f; // Gap between tiles horizontally and vertically
    public TileController tilePrefab;
    private Board board;
    private readonly Dictionary<Vector2Int, TileController> tileControllers = new();
    private int maxIterations = 20;
    
    // Collapse animation settings
    public float collapseDuration = 0.3f; // Duration of collapse animation
    public float collapseStaggerDelay = 0.02f; // Delay between tiles falling
    public bool useScaleEffect = true; // Add pop effect during collapse

    void Start()
    {
        // Example board size, adjust as needed
        int width = 5;
        int height = 5;
        board = new Board(width, height);
        board.Populate();
        GenerateBoard();
        
        // Subscribe to player swipe events
        TileController.PlayerSwiped += OnPlayerSwiped;
    }

    private void OnDestroy()
    {
        // Unsubscribe from player swipe events
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

                    // Store the tile controller reference
                    tileControllers[new Vector2Int(x, y)] = tileObj;

                    // Set tile color based on ID
                    tileObj.SetTileColor(GetColorForTileId(tile.id));
                }
            }
        }
    }

    private Color GetColorForTileId(int tileId)
    {
        return tileId switch
        {
            1 => Color.blue,
            2 => Color.green,
            3 => Color.yellow,
            _ => Color.white
        };
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
        // Wait for swipe animation to complete
        yield return new WaitForSeconds(0.3f);

        // Detect matches
        var matches = board.DetectMatch();

        if (matches.Count > 0)
        {
            Debug.Log($"Matches detected at {matches.Count} positions");
            
            // Clear matches in board and destroy tile objects
            ClearMatches(matches);

            // Wait a bit before collapsing
            yield return new WaitForSeconds(0.2f);

            // Collapse the board with animation
            yield return StartCoroutine(CollapseBoard());

            // Wait before refilling
            yield return new WaitForSeconds(0.2f);

            // Refill the board and check for cascading matches
            yield return StartCoroutine(ContinuousMatchClear());
        }
    }

    private IEnumerator ContinuousMatchClear()
    {
        // Keep clearing matches until there are none left
        bool hasMatches = true;
        
        int iteration = 0;

        while (hasMatches && iteration < maxIterations)
        {
            iteration++;

            // Refill the board with new tiles spawned above
            RefillBoard();

            // Animate the refill falling down
            yield return StartCoroutine(AnimateRefill());

            // Check for matches in the refilled board
            var matches = board.DetectMatch();

            if (matches.Count > 0)
            {
                Debug.Log($"Cascade: Matches detected at {matches.Count} positions (iteration {iteration})");
                
                // Clear the matched tiles
                ClearMatches(matches);

                // Wait before collapsing
                yield return new WaitForSeconds(0.2f);

                // Collapse the board
                yield return StartCoroutine(CollapseBoard());

                // Wait before next check
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                // No more matches found
                hasMatches = false;
                board.Print();
                Debug.Log("No more cascading matches. Board is stable.");
            }
        }

        if (iteration >= maxIterations)
        {
            Debug.LogWarning("Max iterations reached in cascade clearing. Board may be unstable.");
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

    private IEnumerator CollapseBoard()
    {
        // Collapse in the board model
        board.CollapseColumns();

        // Animate tiles falling down
        yield return StartCoroutine(AnimateCollapse());
    }

    private IEnumerator AnimateCollapse()
    {
        // Create a list to track which tiles need to move
        List<(TileController tile, Vector2Int pos, Vector3 startPos, Vector3 targetPos, float staggerTime)> tileMoves = new();

        // Calculate where each tile should move
        foreach (var kvp in new Dictionary<Vector2Int, TileController>(tileControllers))
        {
            Vector2Int pos = kvp.Key;
            TileController tileObj = kvp.Value;

            // Find where this tile should be now after collapse
            Vector3 newPos = transform.position + new Vector3(pos.x * (1f + gap), -pos.y * (1f + gap), 0);
            
            if (tileObj.transform.position != newPos)
            {
                // Calculate stagger delay based on column (tiles fall left to right)
                float staggerTime = pos.x * collapseStaggerDelay;
                tileMoves.Add((tileObj, pos, tileObj.transform.position, newPos, staggerTime));
            }
        }

        // Animate all tiles to their new positions with stagger effect
        float maxStaggerTime = tileMoves.Count > 0 ? tileMoves.Max(t => t.staggerTime) : 0f;
        float totalDuration = maxStaggerTime + collapseDuration;
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            foreach (var (tile, pos, startPos, targetPos, staggerTime) in tileMoves)
            {
                // Only start animating this tile after its stagger delay
                if (elapsed >= staggerTime)
                {
                    float animationElapsed = elapsed - staggerTime;
                    float t = Mathf.Min(animationElapsed / collapseDuration, 1f);
                    
                    // Use ease-out curve for more natural fall (easeOutQuad)
                    float easedT = 1f - (1f - t) * (1f - t);

                    Vector3 newPos = Vector3.Lerp(startPos, targetPos, easedT);

                    // Apply scale effect if enabled
                    if (useScaleEffect)
                    {
                        // Bounce effect: scale up slightly then back to normal
                        float scaleT = Mathf.Min(t * 1.5f, 1f); // Overshoot slightly
                        float scale = 1f + Mathf.Sin(scaleT * Mathf.PI) * 0.1f; // Oscillate between 1 and 1.1
                        tile.transform.localScale = Vector3.one * scale;
                    }

                    tile.transform.position = newPos;
                }
            }

            yield return null;
        }

        // Ensure final positions and scales are exact
        foreach (var (tile, pos, startPos, targetPos, staggerTime) in tileMoves)
        {
            tile.transform.position = targetPos;
            tile.transform.localScale = Vector3.one;
        }

        // Update tile colors to match board state after collapse
        UpdateAllTileColors();
    }

    private void UpdateAllTileColors()
    {
        foreach (var kvp in tileControllers)
        {
            Vector2Int pos = kvp.Key;
            TileController tileObj = kvp.Value;
            
            Tile tile = board.GetTileAtPosition(pos);
            if (tile != null)
            {
                tileObj.SetTileColor(GetColorForTileId(tile.id));
            }
        }
    }

    private void RefillBoard()
    {
        // Refill the board with new tiles
        board.Refill();

        // Create new tile GameObjects for empty positions, spawning them above the board
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Vector2Int pos = new(x, y);

                // If there's no tile object at this position, create one
                if (!tileControllers.ContainsKey(pos))
                {
                    Tile tile = board.GetTileAtPosition(pos);
                    if (tile != null)
                    {
                        // Spawn tile above the board (off-screen vertically)
                        Vector3 spawnPosition = transform.position + new Vector3(x * (1f + gap), board.Height * (1f + gap), 0);

                        // Instantiate tile prefab as child of BoardController
                        TileController tileObj = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, transform);

                        // Store the tile controller reference
                        tileControllers[pos] = tileObj;

                        // Set tile color based on ID
                        tileObj.SetTileColor(GetColorForTileId(tile.id));
                    }
                }
            }
        }
    }

    private IEnumerator AnimateRefill()
    {
        // Create a list to track which tiles need to fall
        List<(TileController tile, Vector2Int pos, Vector3 targetPos)> tileFalls = new();

        // Calculate where each tile should fall to
        foreach (var kvp in new Dictionary<Vector2Int, TileController>(tileControllers))
        {
            Vector2Int pos = kvp.Key;
            TileController tileObj = kvp.Value;

            // Calculate the final position for this tile
            Vector3 finalPos = transform.position + new Vector3(pos.x * (1f + gap), -pos.y * (1f + gap), 0);
            
            // Only animate tiles that are above their final position (newly spawned tiles)
            if (tileObj.transform.position.y > finalPos.y)
            {
                tileFalls.Add((tileObj, pos, finalPos));
            }
        }

        // Animate all falling tiles to their final positions
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration && tileFalls.Count > 0)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            foreach (var (tile, pos, targetPos) in tileFalls)
            {
                tile.transform.position = Vector3.Lerp(tile.transform.position, targetPos, t);
            }

            yield return null;
        }

        // Ensure final positions are exact
        foreach (var (tile, pos, targetPos) in tileFalls)
        {
            tile.transform.position = targetPos;
        }

        // Update tile colors to match board state
        UpdateAllTileColors();
    }
}
