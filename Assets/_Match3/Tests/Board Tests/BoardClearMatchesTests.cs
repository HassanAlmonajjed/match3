using NUnit.Framework;
using UnityEngine;

namespace BoardTests
{
    [TestFixture]
    public class BoardClearMatchesTests
    {
        private Board _board;

        [SetUp]
        public void SetUp()
        {
            _board = new Board(5, 5,4);
        }

        [Test]
        public void ClearMatches_HorizontalMatch_ClearsAllMatchedTiles()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 2, 3},
            {4, 5, 2, 1, 4},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);
            var matches = _board.DetectMatch();

            // Act
            _board.ClearMatches(matches);

            // Assert
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 0)));

            // Non-matched tiles should still exist
            Assert.IsNotNull(_board.GetTileAtPosition(new Vector2Int(3, 0)));
            Assert.IsNotNull(_board.GetTileAtPosition(new Vector2Int(0, 1)));
        }

        [Test]
        public void ClearMatches_VerticalMatch_ClearsAllMatchedTiles()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 2, 3, 4, 5},
            {1, 4, 5, 2, 1},
            {1, 5, 2, 1, 4},
            {2, 1, 4, 5, 2},
            {3, 2, 1, 4, 5}
            };
            _board.Populate(gridIds);
            var matches = _board.DetectMatch();

            // Act
            _board.ClearMatches(matches);

            // Assert
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 1)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 2)));

            // Non-matched tiles should still exist
            Assert.IsNotNull(_board.GetTileAtPosition(new Vector2Int(1, 0)));
            Assert.IsNotNull(_board.GetTileAtPosition(new Vector2Int(0, 3)));
        }

        [Test]
        public void ClearMatches_MultipleMatches_ClearsAllMatchedTiles()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 0, 2},
            {2, 0, 0, 0, 2},
            {3, 3, 3, 0, 2},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);
            var matches = _board.DetectMatch();

            // Act
            _board.ClearMatches(matches);

            // Assert
            // First horizontal match
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 0)));

            // Second horizontal match
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 2)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 2)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 2)));

            // Vertical match
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(4, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(4, 1)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(4, 2)));
        }

        [Test]
        public void ClearMatches_TShapeMatch_ClearsAllMatchedTiles()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 1, 1},
            {0, 0, 1, 0, 0},
            {0, 0, 1, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);
            var matches = _board.DetectMatch();

            // Act
            _board.ClearMatches(matches);

            // Assert
            // Horizontal part
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(3, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(4, 0)));

            // Vertical part
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 1)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 2)));
        }

        [Test]
        public void ClearMatches_LShapeMatch_ClearsAllMatchedTiles()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 0, 0, 0, 0},
            {1, 0, 0, 0, 0},
            {1, 1, 1, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);
            var matches = _board.DetectMatch();

            // Act
            _board.ClearMatches(matches);

            // Assert
            // Vertical part
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 0)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 1)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(0, 2)));

            // Horizontal part
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(1, 2)));
            Assert.IsNull(_board.GetTileAtPosition(new Vector2Int(2, 2)));
        }
    }
}