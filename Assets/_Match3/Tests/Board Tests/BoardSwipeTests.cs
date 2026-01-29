using NUnit.Framework;
using UnityEngine;

namespace BoardTests
{
    public class BoardSwipeTests
    {
        private Board _board;
        private const int Width = 3;
        private const int Height = 3;

        [SetUp]
        public void Setup()
        {
            _board = new Board(Width, Height);
            // Setup a 3x3 board with predictable IDs
            // 1 2 3
            // 4 5 6
            // 7 8 9
            int[,] gridIds = new int[,]
            {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
            };
            _board.Populate(gridIds);
        }

        [Test]
        public void Swipe_Right_SwapsTiles()
        {
            Vector2Int start = new Vector2Int(0, 0); // Tile 1
            Vector2Int end = new Vector2Int(1, 0);   // Tile 2

            Tile tile1 = _board.GetTileAtPosition(start);
            Tile tile2 = _board.GetTileAtPosition(end);

            _board.Swipe(start, end);

            Assert.AreEqual(tile2, _board.GetTileAtPosition(start), "Tile at start should be tile2 after swipe");
            Assert.AreEqual(tile1, _board.GetTileAtPosition(end), "Tile at end should be tile1 after swipe");
        }

        [Test]
        public void Swipe_Left_SwapsTiles()
        {
            Vector2Int start = new Vector2Int(1, 0); // Tile 2
            Vector2Int end = new Vector2Int(0, 0);   // Tile 1

            Tile tile1 = _board.GetTileAtPosition(end);
            Tile tile2 = _board.GetTileAtPosition(start);

            _board.Swipe(start, end);

            Assert.AreEqual(tile1, _board.GetTileAtPosition(start), "Tile at start should be tile1 after swipe");
            Assert.AreEqual(tile2, _board.GetTileAtPosition(end), "Tile at end should be tile2 after swipe");
        }

        [Test]
        public void Swipe_Down_SwapsTiles()
        {
            Vector2Int start = new Vector2Int(0, 0); // Tile 1
            Vector2Int end = new Vector2Int(0, 1);   // Tile 4

            Tile tile1 = _board.GetTileAtPosition(start);
            Tile tile4 = _board.GetTileAtPosition(end);

            _board.Swipe(start, end);

            Assert.AreEqual(tile4, _board.GetTileAtPosition(start), "Tile at start should be tile4 after swipe");
            Assert.AreEqual(tile1, _board.GetTileAtPosition(end), "Tile at end should be tile1 after swipe");
        }

        [Test]
        public void Swipe_Up_SwapsTiles()
        {
            Vector2Int start = new Vector2Int(0, 1); // Tile 4
            Vector2Int end = new Vector2Int(0, 0);   // Tile 1

            Tile tile1 = _board.GetTileAtPosition(end);
            Tile tile4 = _board.GetTileAtPosition(start);

            _board.Swipe(start, end);

            Assert.AreEqual(tile1, _board.GetTileAtPosition(start), "Tile at start should be tile1 after swipe");
            Assert.AreEqual(tile4, _board.GetTileAtPosition(end), "Tile at end should be tile4 after swipe");
        }

        [Test]
        public void Swipe_FromEdge_OutOfBounds_DoesNotThrow_And_NoChange()
        {
            Vector2Int start = new Vector2Int(-1, 0);
            Vector2Int end = new Vector2Int(0, 0);

            // Capture state before swipe
            Tile tile00 = _board.GetTileAtPosition(end);

            Assert.DoesNotThrow(() => _board.Swipe(start, end));

            Assert.AreEqual(tile00, _board.GetTileAtPosition(end), "Tile at (0,0) should not change");
        }

        [Test]
        public void Swipe_ToOutOfBounds_DoesNotThrow_And_NoChange()
        {
            Vector2Int start = new Vector2Int(0, 0);
            Vector2Int end = new Vector2Int(-1, 0);

            // Capture state before swipe
            Tile tile00 = _board.GetTileAtPosition(start);

            Assert.DoesNotThrow(() => _board.Swipe(start, end));

            Assert.AreEqual(tile00, _board.GetTileAtPosition(start), "Tile at (0,0) should not change");
        }

        [Test]
        public void Swipe_AcrossEdges_Right_Boundary()
        {
            Vector2Int start = new Vector2Int(2, 1); // Last column
            Vector2Int end = new Vector2Int(3, 1);   // Out of bounds

            Tile tile21 = _board.GetTileAtPosition(start);

            _board.Swipe(start, end);

            Assert.AreEqual(tile21, _board.GetTileAtPosition(start), "Tile should not move if end is out of bounds");
        }
    }
}