using NUnit.Framework;
using UnityEngine;

namespace BoardTests
{
    public class BoardGetTileAtPositionTests
    {
        [Test]
        public void GetTileAtPosition_WithinBounds_ReturnsTile()
        {
            var board = new Board(5, 5);
            board.Populate();

            Vector2Int pos = new Vector2Int(2, 2);
            Tile tile = board.GetTileAtPosition(pos);

            Assert.IsNotNull(tile, "Expected a tile at position within bounds");
        }

        [Test]
        public void GetTileAtPosition_NegativeCoordinates_ReturnsNull()
        {
            var board = new Board(5, 5);
            board.Populate();

            Vector2Int pos = new Vector2Int(-1, 0);
            Tile tile = board.GetTileAtPosition(pos);

            Assert.IsNull(tile, "Expected null for negative coordinates");
        }

        [Test]
        public void GetTileAtPosition_TooLargeCoordinates_ReturnsNull()
        {
            var board = new Board(3, 3);
            board.Populate();

            Vector2Int pos = new Vector2Int(3, 3);
            Tile tile = board.GetTileAtPosition(pos);

            Assert.IsNull(tile, "Expected null for coordinates outside the board");
        }
    }
}