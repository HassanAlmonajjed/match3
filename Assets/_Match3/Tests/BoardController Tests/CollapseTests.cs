using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

namespace BoardControllerTests
{
    public class CollapseTests
    {
        private BoardController _boardController;
        private Board _board;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject("BoardController");
            _boardController = go.AddComponent<BoardController>();
            
            // Initialize board
            _board = new Board(5, 5);
            _boardController.board = _board;
        }

        [TearDown]
        public void TearDown()
        {
            if (_boardController != null)
            {
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
        public IEnumerator Collapse_UpdatesTileControllerPositionsCorrectly_AfterHorizontalMatch()
        {
            // Arrange: 
            // Create a grid where row 4 (bottom) has a match of 3.
            // Tiles above them should fall down.
            /*
                Grid:
                (0,2): ID 4 (tc1)
                (0,3): ID 5 (tc2)
                (0,4): ID 1 (match)

                (1,3): ID 6 (tc3)
                (1,4): ID 1 (match)

                (2,4): ID 1 (match)
            */

            int[,] gridData = new int[5, 5];
            // Match at row 4 (bottom)
            gridData[4, 0] = 1;
            gridData[4, 1] = 1;
            gridData[4, 2] = 1;
            
            // Tiles to fall
            gridData[2, 0] = 4; // x=0, y=2
            gridData[3, 0] = 5; // x=0, y=3
            gridData[3, 1] = 6; // x=1, y=3
            
            _board.Populate(gridData);

            // Manually populate TileControllers
            Dictionary<Vector2Int, TileController> initialControllers = new();
            
            // x=0 column
            initialControllers[new Vector2Int(0, 2)] = CreateTileController("tc_0_2");
            initialControllers[new Vector2Int(0, 3)] = CreateTileController("tc_0_3");
            initialControllers[new Vector2Int(0, 4)] = CreateTileController("match_0_4");
            
            // x=1 column
            initialControllers[new Vector2Int(1, 3)] = CreateTileController("tc_1_3");
            initialControllers[new Vector2Int(1, 4)] = CreateTileController("match_1_4");
            
            // x=2 column
            initialControllers[new Vector2Int(2, 4)] = CreateTileController("match_2_4");

            // Fill the BoardController.Tiles dictionary
            foreach (var kvp in initialControllers)
            {
                _boardController.Tiles.Add(kvp.Key, kvp.Value);
            }

            // Act:
            // 1. Detect and clear matches
            var matches = _board.DetectMatch();
            Assert.AreEqual(3, matches.Count, "Should detect 3 matching tiles");
            _boardController.ClearMatches(matches);

            // 2. Collapse Columns in Board model
            _board.CollapseColumns();

            // 3. Collapse in BoardController (returns new dictionary)
            var newTiles = _boardController.Collapse();

            // Assert:
            // Column 0: (0,2) and (0,3) should fall to (0,3) and (0,4)
            Assert.IsTrue(newTiles.ContainsKey(new Vector2Int(0, 3)), "Should have tile at (0,3)");
            Assert.IsTrue(newTiles.ContainsKey(new Vector2Int(0, 4)), "Should have tile at (0,4)");
            Assert.AreEqual(initialControllers[new Vector2Int(0, 2)], newTiles[new Vector2Int(0, 3)], "tc_0_2 should fall to (0,3)");
            Assert.AreEqual(initialControllers[new Vector2Int(0, 3)], newTiles[new Vector2Int(0, 4)], "tc_0_3 should fall to (0,4)");

            // Column 1: (1,3) should fall to (1,4)
            Assert.IsTrue(newTiles.ContainsKey(new Vector2Int(1, 4)), "Should have tile at (1,4)");
            Assert.AreEqual(initialControllers[new Vector2Int(1, 3)], newTiles[new Vector2Int(1, 4)], "tc_1_3 should fall to (1,4)");

            // Column 2: Was (2,4), now empty in Board but Collapse() only handles existing controllers
            Assert.IsFalse(newTiles.ContainsKey(new Vector2Int(2, 4)), "Column 2 should be empty after collapse before refill");

            yield return null;
        }

        [UnityTest]
        public IEnumerator Collapse_UpdatesTileControllerPositionsCorrectly_AfterVerticalMatch()
        {
            // Arrange:
            // Vertical match at x=0, y=2,3,4.
            // Tile at (0,0) and (0,1) should fall 3 steps.
            
            int[,] gridData = new int[5, 5];
            gridData[0, 0] = 5; // Falls to (0,3)
            gridData[1, 0] = 6; // Falls to (0,4)
            gridData[2, 0] = 1; // Match
            gridData[3, 0] = 1; // Match
            gridData[4, 0] = 1; // Match
            
            _board.Populate(gridData);

            Dictionary<Vector2Int, TileController> initialControllers = new();
            initialControllers[new Vector2Int(0, 0)] = CreateTileController("tc_0_0");
            initialControllers[new Vector2Int(0, 1)] = CreateTileController("tc_0_1");
            initialControllers[new Vector2Int(0, 2)] = CreateTileController("match_0_2");
            initialControllers[new Vector2Int(0, 3)] = CreateTileController("match_0_3");
            initialControllers[new Vector2Int(0, 4)] = CreateTileController("match_0_4");

            foreach (var kvp in initialControllers)
            {
                _boardController.Tiles.Add(kvp.Key, kvp.Value);
            }

            // Act:
            var matches = _board.DetectMatch();
            Assert.AreEqual(3, matches.Count, "Should detect 3 matching tiles");
            _boardController.ClearMatches(matches);
            _board.CollapseColumns();
            var newTiles = _boardController.Collapse();

            // Assert:
            Assert.AreEqual(initialControllers[new Vector2Int(0, 0)], newTiles[new Vector2Int(0, 3)], "tc_0_0 should fall to (0,3)");
            Assert.AreEqual(initialControllers[new Vector2Int(0, 1)], newTiles[new Vector2Int(0, 4)], "tc_0_1 should fall to (0,4)");
            Assert.IsFalse(newTiles.ContainsKey(new Vector2Int(0, 0)));
            Assert.IsFalse(newTiles.ContainsKey(new Vector2Int(0, 1)));
            Assert.IsFalse(newTiles.ContainsKey(new Vector2Int(0, 2)));

            yield return null;
        }

        private TileController CreateTileController(string name)
        {
            GameObject go = new GameObject(name);
            return go.AddComponent<TileController>();
        }
    }
}
