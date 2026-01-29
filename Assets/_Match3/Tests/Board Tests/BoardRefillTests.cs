using NUnit.Framework;
using UnityEngine;

namespace BoardTests
{
    [TestFixture]
    public class BoardRefillTests
    {
        private Board _board;
        private const int Width = 5;
        private const int Height = 5;

        [SetUp]
        public void SetUp()
        {
            _board = new Board(Width, Height, 4);
        }

        private void ProcessMatchCollapseAndRefill()
        {
            var matches = _board.DetectMatch();
            _board.ClearMatches(matches);
            _board.CollapseColumns();
            _board.Refill();
        }

        private void AssertBoardIsFull()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Assert.IsNotNull(_board.GetTileAtPosition(new Vector2Int(x, y)), $"Tile at ({x}, {y}) should not be null after refill.");
                }
            }
        }

        [Test]
        public void Refill_AfterHorizontalMatch_BoardIsFull()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 2, 3},
            {4, 5, 2, 1, 4},
            {2, 3, 1, 4, 5},
            {1, 2, 3, 4, 5},
            {5, 4, 3, 2, 1}
            };
            _board.Populate(gridIds);

            // Act
            ProcessMatchCollapseAndRefill();

            // Assert
            AssertBoardIsFull();
        }

        [Test]
        public void Refill_AfterVerticalMatch_BoardIsFull()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 2, 3, 4, 5},
            {1, 3, 4, 5, 2},
            {1, 4, 5, 2, 3},
            {2, 5, 2, 3, 4},
            {3, 1, 3, 4, 1}
            };
            _board.Populate(gridIds);

            // Act
            ProcessMatchCollapseAndRefill();

            // Assert
            AssertBoardIsFull();
        }

        [Test]
        public void Refill_AfterLShapeMatch_BoardIsFull()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 4, 5},
            {1, 3, 4, 5, 2},
            {1, 4, 5, 2, 3},
            {2, 5, 2, 3, 4},
            {3, 1, 3, 4, 1}
            };
            _board.Populate(gridIds);

            // Act
            ProcessMatchCollapseAndRefill();

            // Assert
            AssertBoardIsFull();
        }

        [Test]
        public void Refill_AfterTShapeMatch_BoardIsFull()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 1, 1},
            {3, 3, 1, 3, 3},
            {4, 4, 1, 4, 4},
            {2, 5, 2, 3, 4},
            {3, 1, 3, 4, 1}
            };
            _board.Populate(gridIds);

            // Act
            ProcessMatchCollapseAndRefill();

            // Assert
            AssertBoardIsFull();
        }

        [Test]
        public void Refill_AfterCrossingMatches_BoardIsFull()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 2, 3, 4, 5},
            {1, 2, 3, 4, 5},
            {2, 2, 2, 2, 2},
            {1, 2, 3, 4, 5},
            {1, 2, 3, 4, 5}
            };
            _board.Populate(gridIds);

            // Act
            ProcessMatchCollapseAndRefill();

            // Assert
            AssertBoardIsFull();
        }

        [Test]
        public void Refill_AfterClearingAlmostEntireBoard_BoardIsFull()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 2, 2},
            {1, 1, 1, 2, 2},
            {1, 1, 1, 2, 2},
            {3, 3, 3, 4, 4},
            {3, 3, 3, 4, 4}
            };
            _board.Populate(gridIds);

            // Act
            ProcessMatchCollapseAndRefill();

            // Assert
            AssertBoardIsFull();
        }

        [Test]
        public void Refill_EmptyBoard_BoardIsFull()
        {
            // Arrange - All nulls
            // No need to populate, new Board(W, H) has nulls by default but Populate() initializes with random if no args.
            // Actually, the constructor doesn't initialize _grid elements, so they are null.

            // Act
            _board.Refill();

            // Assert
            AssertBoardIsFull();
        }

        [Test]
        public void Refill_DoesNotFillExcludedTiles()
        {
            int width = 3;
            int height = 3;
            var excluded = new System.Collections.Generic.List<Vector2Int> { new Vector2Int(1, 1) };

            var board = new Board(width, height, 4, excluded);
            // Don't call populate, so all tiles are null
            
            board.Refill();

            Assert.IsNull(board.GetTileAtPosition(new Vector2Int(1, 1)), "Excluded tile should not be refilled");
            Assert.IsNotNull(board.GetTileAtPosition(new Vector2Int(0, 0)), "Normal tile should be refilled");
        }

        /*[Test]
        public void Refill_ReturnsCorrectPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
                {1, 1, 0, 2, 3},
                {4, 0, 2, 1, 4},
                {2, 3, 1, 4, 5},
                {1, 2, 3, 4, 5},
                {5, 4, 3, 2, 1}
            };
            _board.Populate(gridIds);
            
            // Act
            _board.Refill();

            // Assert
            Assert.AreEqual(2, refillData.Count, "Should have 2 refilled tiles.");
            
            var tile1 = refillData.Find(t => t.endPos == new Vector2Int(1, 1));
            Assert.AreEqual(new Vector2Int(1, -1), tile1.startPos);

            var tile2 = refillData.Find(t => t.endPos == new Vector2Int(2, 0));
            Assert.AreEqual(new Vector2Int(2, -1), tile2.startPos);
        }*/
    }
}