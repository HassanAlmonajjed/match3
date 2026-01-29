using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BoardControllerTests
{
    public class ClearMatchesTests
    {
        private BoardController _boardController;
        private Board _board;
        private TileController _tilePrefab;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject("BoardController");
            _boardController = go.AddComponent<BoardController>();
            
            // Initialize board
            _board = new Board(5, 5, 4);
            _boardController.board = _board;
        }

        [TearDown]
        public void TearDown()
        {
            if (_boardController != null)
            {
                // Clean up any remaining tiles in the dictionary
                foreach (var tile in _boardController.Tiles.Values)
                {
                    if (tile != null)
                    {
                        Object.DestroyImmediate(tile.gameObject);
                    }
                }
                _boardController.Tiles.Clear();
                Object.DestroyImmediate(_boardController.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator ClearMatches_RemovesTilesFromBoardAndDictionary()
        {
            // Arrange: Setup a match of 3 tiles
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            Vector2Int pos3 = new Vector2Int(2, 0);

            // Manually populate the board model
            int[,] gridData = new int[5, 5];
            gridData[0, 0] = 1;
            gridData[0, 1] = 1;
            gridData[0, 2] = 1;
            _board.Populate(gridData);

            // Manually populate the TileController dictionary
            GameObject t1 = new GameObject("Tile_0_0");
            GameObject t2 = new GameObject("Tile_1_0");
            GameObject t3 = new GameObject("Tile_2_0");
            
            TileController tc1 = t1.AddComponent<TileController>();
            TileController tc2 = t2.AddComponent<TileController>();
            TileController tc3 = t3.AddComponent<TileController>();

            _boardController.Tiles.Add(pos1, tc1);
            _boardController.Tiles.Add(pos2, tc2);
            _boardController.Tiles.Add(pos3, tc3);

            HashSet<Vector2Int> matches = new HashSet<Vector2Int> { pos1, pos2, pos3 };

            // Act: Clear the matches
            _boardController.ClearMatches(matches);

            // Assert: Dictionary removal should be immediate
            Assert.IsFalse(_boardController.Tiles.ContainsKey(pos1), "Pos1 should be removed from dictionary");
            Assert.IsFalse(_boardController.Tiles.ContainsKey(pos2), "Pos2 should be removed from dictionary");
            Assert.IsFalse(_boardController.Tiles.ContainsKey(pos3), "Pos3 should be removed from dictionary");

            // Assert: Board model update should be immediate
            Assert.IsNull(_board.GetTileAtPosition(pos1), "Board at Pos1 should be null");
            Assert.IsNull(_board.GetTileAtPosition(pos2), "Board at Pos2 should be null");
            Assert.IsNull(_board.GetTileAtPosition(pos3), "Board at Pos3 should be null");

            // Assert: GameObjects should be destroyed (null in Unity)
            Assert.IsTrue(t1 == null || t1.Equals(null));
            Assert.IsTrue(t2 == null || t2.Equals(null));
            Assert.IsTrue(t3 == null || t3.Equals(null));

            yield return null;
        }

        [UnityTest]
        public IEnumerator ClearMatches_DoesNotEffectNonMatchingTiles()
        {
            // Arrange
            Vector2Int matchPos = new Vector2Int(0, 0);
            Vector2Int nonMatchPos = new Vector2Int(4, 4);

            int[,] gridData = new int[5, 5];
            gridData[0, 0] = 1;
            gridData[4, 4] = 2;
            _board.Populate(gridData);

            GameObject t1 = new GameObject("MatchTile");
            GameObject t2 = new GameObject("NonMatchTile");
            
            TileController tc1 = t1.AddComponent<TileController>();
            TileController tc2 = t2.AddComponent<TileController>();

            _boardController.Tiles.Add(matchPos, tc1);
            _boardController.Tiles.Add(nonMatchPos, tc2);

            HashSet<Vector2Int> matches = new HashSet<Vector2Int> { matchPos };

            // Act
            _boardController.ClearMatches(matches);

            // Assert
            Assert.IsFalse(_boardController.Tiles.ContainsKey(matchPos), "Matched tile should be removed from dictionary");
            Assert.IsTrue(_boardController.Tiles.ContainsKey(nonMatchPos), "Non-matching tile should remain in dictionary");
            
            Assert.IsNull(_board.GetTileAtPosition(matchPos), "Matched tile should be null in board model");
            Assert.IsNotNull(_board.GetTileAtPosition(nonMatchPos), "Non-matching tile should remain in board model");

            Assert.IsTrue(t1 == null, "Matched GameObject should be destroyed");
            Assert.IsFalse(t2 == null, "Non-matching GameObject should NOT be destroyed");

            yield return null;
        }
    }
}
