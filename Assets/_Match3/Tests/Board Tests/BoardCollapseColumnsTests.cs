using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[TestFixture]
public class BoardCollapseColumnsTests
{
    private Board _board;

    [SetUp]
    public void SetUp()
    {
        _board = new Board(5, 5);
    }

    [Test]
    public void CollapseColumns_SingleEmptySpace_TileMovesDown()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {1, 0, 0, 0, 0},
            {2, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {3, 0, 0, 0, 0},
            {4, 0, 0, 0, 0}
        };
        _board.Populate(gridIds);

        // Clear the middle tile to create empty space
        _board.ClearMatches(new HashSet<Vector2Int> { new Vector2Int(0, 2) });

        // Act
        _board.CollapseColumns();

        // Assert - Corrected for top-left origin: accumulation at y=4
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
        Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(0, 1)).id);
        Assert.AreEqual(2, _board.GetTileAtPosition(new Vector2Int(0, 2)).id);
        Assert.AreEqual(3, _board.GetTileAtPosition(new Vector2Int(0, 3)).id);
        Assert.AreEqual(4, _board.GetTileAtPosition(new Vector2Int(0, 4)).id);
    }

    [Test]
    public void CollapseColumns_MultipleEmptySpaces_TilesMoveToBottom()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {2, 0, 0, 0, 0},
            {3, 0, 0, 0, 0}
        };
        _board.Populate(gridIds);

        // Act
        _board.CollapseColumns();

        // Assert - Corrected for top-left origin: accumulation at y=4
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 1)));
        Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(0, 2)).id);
        Assert.AreEqual(2, _board.GetTileAtPosition(new Vector2Int(0, 3)).id);
        Assert.AreEqual(3, _board.GetTileAtPosition(new Vector2Int(0, 4)).id);
    }

    [Test]
    public void CollapseColumns_EmptyColumn_NoChange()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {0, 1, 2, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
        };
        _board.Populate(gridIds);

        // Act
        _board.CollapseColumns();

        // Assert - Column 0 should remain empty, other columns should fall to bottom (y=4)
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 1)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 2)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 3)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 4)));
        
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 1)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 2)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 3)));
        Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(1, 4)).id);

        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 1)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 2)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 3)));
        Assert.AreEqual(2, _board.GetTileAtPosition(new Vector2Int(2, 4)).id);
    }

    [Test]
    public void CollapseColumns_FullColumn_NoChange()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {1, 2, 3, 4, 5},
            {1, 2, 3, 4, 5},
            {1, 2, 3, 4, 5},
            {1, 2, 3, 4, 5},
            {1, 2, 3, 4, 5}
        };
        _board.Populate(gridIds);

        // Act
        _board.CollapseColumns();

        // Assert - All tiles should remain in their original positions
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Assert.IsNotNull(_board.GetTileAtPosition(new Vector2Int(x, y)));
                Assert.AreEqual(x + 1, _board.GetTileAtPosition(new Vector2Int(x, y)).id);
            }
        }
    }

    [Test]
    public void CollapseColumns_MultipleColumnsWithGaps_AllCollapseCorrectly()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {1, 0, 2, 0, 3},
            {0, 0, 0, 0, 0},
            {4, 5, 6, 0, 0},
            {0, 0, 0, 7, 0},
            {8, 0, 9, 0, 10}
        };
        _board.Populate(gridIds);

        // Act
        _board.CollapseColumns();

        // Assert - Column 0: 1,4,8 should collapse to bottom
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 1)));
        Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(0, 2)).id);
        Assert.AreEqual(4, _board.GetTileAtPosition(new Vector2Int(0, 3)).id);
        Assert.AreEqual(8, _board.GetTileAtPosition(new Vector2Int(0, 4)).id);
        
        // Assert - Column 1: 5 should fall to bottom
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 1)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 2)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 3)));
        Assert.AreEqual(5, _board.GetTileAtPosition(new Vector2Int(1, 4)).id);
        
        // Assert - Column 2: 2,6,9 should collapse to bottom
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 1)));
        Assert.AreEqual(2, _board.GetTileAtPosition(new Vector2Int(2, 2)).id);
        Assert.AreEqual(6, _board.GetTileAtPosition(new Vector2Int(2, 3)).id);
        Assert.AreEqual(9, _board.GetTileAtPosition(new Vector2Int(2, 4)).id);
        
        // Assert - Column 3: 7 should fall to bottom
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(3, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(3, 1)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(3, 2)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(3, 3)));
        Assert.AreEqual(7, _board.GetTileAtPosition(new Vector2Int(3, 4)).id);
        
        // Assert - Column 4: 3,10 should collapse to bottom
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(4, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(4, 1)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(4, 2)));
        Assert.AreEqual(3, _board.GetTileAtPosition(new Vector2Int(4, 3)).id);
        Assert.AreEqual(10, _board.GetTileAtPosition(new Vector2Int(4, 4)).id);
    }

    [Test]
    public void CollapseColumns_AlternatingPattern_CollapsesCorrectly()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {1, 0, 1, 0, 1},
            {0, 1, 0, 1, 0},
            {1, 0, 1, 0, 1},
            {0, 1, 0, 1, 0},
            {1, 0, 1, 0, 1}
        };
        _board.Populate(gridIds);

        // Act
        _board.CollapseColumns();

        for (int x = 0; x < 5; x++)
        {
            if (x % 2 == 0) // Even columns (0,2,4) had 3 tiles
            {
                Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 0)));
                Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 1)));
                Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(x, 2)).id);
                Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(x, 3)).id);
                Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(x, 4)).id);
            }
            else // Odd columns (1,3) had 2 tiles
            {
                Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 0)));
                Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 1)));
                Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 2)));
                Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(x, 3)).id);
                Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(x, 4)).id);
            }
        }
    }

    [Test]
    public void CollapseColumns_SingleTileAtTop_FallsToBottom()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
        };
        _board.Populate(gridIds);

        // Act
        _board.CollapseColumns();

        // Assert
        // Assert - Corrected for top-left origin: accumulation at y=4
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 1)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 2)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 3)));
        Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(0, 4)).id);
    }

    [Test]
    public void CollapseColumns_AllTilesAtBottom_NoChange()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {1, 2, 3, 4, 5},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
        };
        _board.Populate(gridIds);
        
        // Act
        _board.CollapseColumns();

        // Assert - Corrected for top-left origin: accumulation at bottom (y=4)
        for (int x = 0; x < 5; x++)
        {
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 1)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 2)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(x, 3)));
            Assert.AreEqual(x + 1, _board.GetTileAtPosition(new Vector2Int(x, 4)).id);
        }
    }

    [Test]
    public void CollapseColumns_LargeGap_TilesFallCorrectly()
    {
        // Arrange
        var gridIds = new int[,]
        {
            {1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {2, 0, 0, 0, 0}
        };
        _board.Populate(gridIds);

        // Act
        _board.CollapseColumns();

        // Assert - Corrected for top-left origin: accumulation at y=4
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 1)));
        Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 2)));
        Assert.AreEqual(1, _board.GetTileAtPosition(new Vector2Int(0, 3)).id);
        Assert.AreEqual(2, _board.GetTileAtPosition(new Vector2Int(0, 4)).id);
    }
}
