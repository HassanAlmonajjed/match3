using NUnit.Framework;
using UnityEngine;

public class BoardPopulateTests
{
    [Test]
    public void Populate_FillsGrid_And_NoInitialMatches_MultipleRuns()
    {
        int width = 6;
        int height = 6;
        int runs = 10; 

        for (int i = 0; i < runs; i++)
        {
            var board = new Board(width, height);
            board.Populate();

            // Verify grid is populated
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = board.GetTileAtPosition(new Vector2Int(x, y));
                    Assert.IsNotNull(tile,
                        $"Run {i}: Expected tile at ({x},{y})");
                }
            }

            // Verify no matches
            var matches = board.DetectMatch();
            Assert.AreEqual(0, matches.Count,
                $"Run {i}: Found {matches.Count} initial matches");
        }
    }

    [Test]
    public void Populate_WithGivenGrid_SetsTilesCorrectly()
    {
        int width = 3;
        int height = 2;
        int[,] gridIds = new int[,]
        {
            { 1, 2, 3 },
            { 2, 1, 3 }
        };

        var board = new Board(width, height);
        board.Populate(gridIds);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var tile = board.GetTileAtPosition(new Vector2Int(x, y));
                Assert.IsNotNull(tile, $"Expected tile at ({x},{y})");
                Assert.AreEqual(gridIds[y, x], tile.id, $"Tile ID mismatch at ({x},{y})");
            }
        }
    }

    [Test]
    public void Populate_WithPartialGrid_SetsTilesAndLeavesNulls()
    {
        int width = 3;
        int height = 3;
        // 2x2 grid provided for a 3x3 board
        int[,] gridIds = new int[,]
        {
            { 1, 2 },
            { 3, 0 } // 0 should be treated as empty/null based on gridIds[y, x] > 0 check
        };

        var board = new Board(width, height);
        board.Populate(gridIds);

        // Check (0,0) -> 1
        Assert.AreEqual(1, board.GetTileAtPosition(new Vector2Int(0, 0)).id);
        // Check (1,0) -> 2
        Assert.AreEqual(2, board.GetTileAtPosition(new Vector2Int(1, 0)).id);
        // Check (0,1) -> 3
        Assert.AreEqual(3, board.GetTileAtPosition(new Vector2Int(0, 1)).id);
        
        // Check (1,1) -> 0 -> should be null
        Assert.IsNull(board.GetTileAtPosition(new Vector2Int(1, 1)), "Expected null tile at (1,1) because ID was 0");
        
        // Check (2,0) -> out of bounds of gridIds width -> should be null
        Assert.IsNull(board.GetTileAtPosition(new Vector2Int(2, 0)), "Expected null at (2,0)");

        // Check (0,2) -> out of bounds of gridIds height -> should be null
        Assert.IsNull(board.GetTileAtPosition(new Vector2Int(0, 2)), "Expected null at (0,2)");
    }
}
