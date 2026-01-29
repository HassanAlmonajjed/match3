using NUnit.Framework;
using UnityEngine;

namespace BoardTests
{
    [TestFixture]
    public class BoardDetectMatchTests
    {
        private Board _board;

        [SetUp]
        public void SetUp()
        {
            _board = new Board(5, 5, 4);
        }

        [Test]
        public void DetectMatch_HorizontalMatchOfThree_ReturnsCorrectPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 2, 3},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(3, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 0)));
        }

        [Test]
        public void DetectMatch_HorizontalMatchOfFour_ReturnsCorrectPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {0, 0, 0, 0, 0},
            {2, 2, 2, 2, 1},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(4, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 1)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 1)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 1)));
            Assert.IsTrue(matches.Contains(new Vector2Int(3, 1)));
        }

        [Test]
        public void DetectMatch_HorizontalMatchOfFive_ReturnsCorrectPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {3, 3, 3, 3, 3},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(5, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(3, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(4, 2)));
        }

        [Test]
        public void DetectMatch_VerticalMatchOfThree_ReturnsCorrectPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 0, 0, 0, 0},
            {1, 0, 0, 0, 0},
            {1, 0, 0, 0, 0},
            {2, 0, 0, 0, 0},
            {3, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(3, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 1)));
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 2)));
        }

        [Test]
        public void DetectMatch_VerticalMatchOfFour_ReturnsCorrectPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {0, 2, 0, 0, 0},
            {0, 2, 0, 0, 0},
            {0, 2, 0, 0, 0},
            {0, 2, 0, 0, 0},
            {0, 1, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(4, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 1)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 3)));
        }

        [Test]
        public void DetectMatch_VerticalMatchOfFive_ReturnsCorrectPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {0, 0, 3, 0, 0},
            {0, 0, 3, 0, 0},
            {0, 0, 3, 0, 0},
            {0, 0, 3, 0, 0},
            {0, 0, 3, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(5, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 1)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 3)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 4)));
        }

        [Test]
        public void DetectMatch_MultipleHorizontalMatches_ReturnsAllPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 0, 0},
            {0, 0, 0, 0, 0},
            {2, 2, 2, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(6, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 2)));
        }

        [Test]
        public void DetectMatch_MultipleVerticalMatches_ReturnsAllPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 0, 2, 0, 0},
            {1, 0, 2, 0, 0},
            {1, 0, 2, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(6, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 1)));
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 2)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 1)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 2)));
        }

        [Test]
        public void DetectMatch_HorizontalAndVerticalMatches_ReturnsAllPositions()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 1, 3, 4},
            {1, 0, 0, 0, 0},
            {1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(5, matches.Count);
        }

        [Test]
        public void DetectMatch_NoMatches_ReturnsEmptySet()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 2, 3, 0, 0},
            {2, 0, 0, 0, 0},
            {3, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void DetectMatch_TwoInARow_ReturnsEmptySet()
        {
            // Arrange
            var gridIds = new int[,]
            {
            {1, 1, 2, 0, 0},
            {1, 0, 0, 0, 0},
            {2, 0, 0, 0, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
            };
            _board.Populate(gridIds);

            // Act
            var matches = _board.DetectMatch();

            // Assert
            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void DetectMatch_LShapeMatch_ReturnsOnlyStraightLines()
        {
            // Arrange
            var board = new Board(3, 3, 4);
            var gridIds = new int[,]
            {
            {1, 1, 1},
            {1, 0, 0},
            {2, 0, 0}
            };
            board.Populate(gridIds);

            // Act
            var matches = board.DetectMatch();

            // Assert
            Assert.AreEqual(3, matches.Count);
            Assert.IsTrue(matches.Contains(new Vector2Int(0, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(1, 0)));
            Assert.IsTrue(matches.Contains(new Vector2Int(2, 0)));
        }
    }
}