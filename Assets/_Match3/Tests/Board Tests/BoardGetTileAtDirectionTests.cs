using NUnit.Framework;
using UnityEngine;

namespace BoardTests
{
    public class BoardGetTileAtDirectionTests
    {
        private Board _board;
        private const int Width = 3;
        private const int Height = 3;

        [SetUp]
        public void Setup()
        {
            _board = new Board(Width, Height);
            // Setup a 3x3 board
            // (0,0) (1,0) (2,0)
            // (0,1) (1,1) (2,1)
            // (0,2) (1,2) (2,2)
            int[,] gridIds = new int[,]
            {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
            };
            _board.Populate(gridIds);
        }

        [Test]
        public void GetTileAtDirection_Up_ReturnsCorrectPosition()
        {
            Vector2Int start = new Vector2Int(1, 1);
            Vector2Int expected = new Vector2Int(1, 0);
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Up);
            Assert.AreEqual(expected, result, "Up from (1,1) should be (1,0)");
        }

        [Test]
        public void GetTileAtDirection_Down_ReturnsCorrectPosition()
        {
            Vector2Int start = new Vector2Int(1, 1);
            Vector2Int expected = new Vector2Int(1, 2);
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Down);
            Assert.AreEqual(expected, result, "Down from (1,1) should be (1,2)");
        }

        [Test]
        public void GetTileAtDirection_Left_ReturnsCorrectPosition()
        {
            Vector2Int start = new Vector2Int(1, 1);
            Vector2Int expected = new Vector2Int(0, 1);
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Left);
            Assert.AreEqual(expected, result, "Left from (1,1) should be (0,1)");
        }

        [Test]
        public void GetTileAtDirection_Right_ReturnsCorrectPosition()
        {
            Vector2Int start = new Vector2Int(1, 1);
            Vector2Int expected = new Vector2Int(2, 1);
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Right);
            Assert.AreEqual(expected, result, "Right from (1,1) should be (2,1)");
        }

        [Test]
        public void GetTileAtDirection_TopEdge_Up_ReturnsInvalid()
        {
            Vector2Int start = new Vector2Int(1, 0);
            Vector2Int expected = new Vector2Int(-1, -1);
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Up);
            Assert.AreEqual(expected, result, "Up from top edge should be (-1,-1)");
        }

        [Test]
        public void GetTileAtDirection_BottomEdge_Down_ReturnsInvalid()
        {
            Vector2Int start = new Vector2Int(1, Height - 1);
            Vector2Int expected = new Vector2Int(-1, -1);
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Down);
            Assert.AreEqual(expected, result, "Down from bottom edge should be (-1,-1)");
        }

        [Test]
        public void GetTileAtDirection_LeftEdge_Left_ReturnsInvalid()
        {
            Vector2Int start = new Vector2Int(0, 1);
            Vector2Int expected = new Vector2Int(-1, -1);
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Left);
            Assert.AreEqual(expected, result, "Left from left edge should be (-1,-1)");
        }

        [Test]
        public void GetTileAtDirection_RightEdge_Right_ReturnsInvalid()
        {
            Vector2Int start = new Vector2Int(Width - 1, 1);
            Vector2Int expected = new Vector2Int(-1, -1);
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Right);
            Assert.AreEqual(expected, result, "Right from right edge should be (-1,-1)");
        }

        [Test]
        public void GetTileAtDirection_MiddleIsNull_ReturnsPositionAndNullTile()
        {
            // Re-populate with a null tile in the middle
            int[,] gridIds = new int[,]
            {
            { 1, 2, 3 },
            { 4, 0, 6 }, // (1,1) is 0 -> null
            { 7, 8, 9 }
            };
            _board.Populate(gridIds);

            Vector2Int start = new Vector2Int(0, 1); // Tile at (0,1) which is 4
            Vector2Int result = _board.GetTileAtDirection(start, SwipeDirection.Right);

            Assert.AreEqual(new Vector2Int(1, 1), result, "Should still return the position (1,1)");
            Assert.IsNull(_board.GetTileAtPosition(result), "Tile at (1,1) should be null");
        }

        [Test]
        public void GetTileAtDirection_Corners_AllDirections()
        {
            Vector2Int invalid = new Vector2Int(-1, -1);

            // Top-Left (0,0)
            Assert.AreEqual(invalid, _board.GetTileAtDirection(new Vector2Int(0, 0), SwipeDirection.Up));
            Assert.AreEqual(invalid, _board.GetTileAtDirection(new Vector2Int(0, 0), SwipeDirection.Left));
            Assert.AreEqual(new Vector2Int(1, 0), _board.GetTileAtDirection(new Vector2Int(0, 0), SwipeDirection.Right));
            Assert.AreEqual(new Vector2Int(0, 1), _board.GetTileAtDirection(new Vector2Int(0, 0), SwipeDirection.Down));

            // Top-Right (2,0)
            Assert.AreEqual(invalid, _board.GetTileAtDirection(new Vector2Int(2, 0), SwipeDirection.Up));
            Assert.AreEqual(invalid, _board.GetTileAtDirection(new Vector2Int(2, 0), SwipeDirection.Right));
            Assert.AreEqual(new Vector2Int(1, 0), _board.GetTileAtDirection(new Vector2Int(2, 0), SwipeDirection.Left));
            Assert.AreEqual(new Vector2Int(2, 1), _board.GetTileAtDirection(new Vector2Int(2, 0), SwipeDirection.Down));

            // Bottom-Left (0,2)
            Assert.AreEqual(invalid, _board.GetTileAtDirection(new Vector2Int(0, 2), SwipeDirection.Down));
            Assert.AreEqual(invalid, _board.GetTileAtDirection(new Vector2Int(0, 2), SwipeDirection.Left));
            Assert.AreEqual(new Vector2Int(1, 2), _board.GetTileAtDirection(new Vector2Int(0, 2), SwipeDirection.Right));
            Assert.AreEqual(new Vector2Int(0, 1), _board.GetTileAtDirection(new Vector2Int(0, 2), SwipeDirection.Up));

            // Bottom-Right (2,2)
            Assert.AreEqual(invalid, _board.GetTileAtDirection(new Vector2Int(2, 2), SwipeDirection.Down));
            Assert.AreEqual(invalid, _board.GetTileAtDirection(new Vector2Int(2, 2), SwipeDirection.Right));
            Assert.AreEqual(new Vector2Int(1, 2), _board.GetTileAtDirection(new Vector2Int(2, 2), SwipeDirection.Left));
            Assert.AreEqual(new Vector2Int(2, 1), _board.GetTileAtDirection(new Vector2Int(2, 2), SwipeDirection.Up));
        }

        [Test]
        public void GetTileAtDirection_None_ReturnsSamePosition()
        {
            Vector2Int start = new Vector2Int(1, 1);
            Assert.AreEqual(start, _board.GetTileAtDirection(start, SwipeDirection.None), "None direction should return the same position");
        }
    }
}