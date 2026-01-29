using NUnit.Framework;
using UnityEngine;

namespace BoardControllerTests
{
    public class CalculateTilePositionTests
    {
        private BoardController _boardController;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject("BoardController");
            _boardController = go.AddComponent<BoardController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_boardController.gameObject);
        }

        [Test]
        public void CalculateTilePosition_Origin_NoGap_NoOffset()
        {
            // Arrange
            int i = 0;
            int j = 0;
            float tileSize = 1.0f;
            float gap = 0f;
            Vector3 offset = Vector3.zero;

            // Act
            Vector3 result = _boardController.CalculateTilePosition(i, j, tileSize, gap, offset);

            // Assert
            Assert.AreEqual(Vector3.zero, result);
        }

        [Test]
        public void CalculateTilePosition_WithGap_NoOffset()
        {
            // Arrange
            int i = 1;
            int j = 1;
            float tileSize = 1.0f;
            float gap = 0.5f;
            Vector3 offset = Vector3.zero;

            // Act
            Vector3 result = _boardController.CalculateTilePosition(i, j, tileSize, gap, offset);

            // Assert
            // x = 1 * (1.0 + 0.5) = 1.5
            // y = -1 * (1.0 + 0.5) = -1.5
            Assert.AreEqual(new Vector3(1.5f, -1.5f, 0f), result);
        }

        [Test]
        public void CalculateTilePosition_WithOffset_NoGap()
        {
            // Arrange
            int i = 2;
            int j = 3;
            float tileSize = 1.0f;
            float gap = 0f;
            Vector3 offset = new Vector3(10f, 20f, 5f);

            // Act
            Vector3 result = _boardController.CalculateTilePosition(i, j, tileSize, gap, offset);

            // Assert
            // x = 10 + 2 * 1 = 12
            // y = 20 + (-3 * 1) = 17
            // z = 5
            Assert.AreEqual(new Vector3(12f, 17f, 5f), result);
        }

        [Test]
        public void CalculateTilePosition_WithEverything()
        {
            // Arrange
            int i = 2;
            int j = 2;
            float tileSize = 2.0f;
            float gap = 1.0f;
            Vector3 offset = new Vector3(-5f, -5f, 0f);

            // Act
            Vector3 result = _boardController.CalculateTilePosition(i, j, tileSize, gap, offset);

            // Assert
            // x = -5 + 2 * (2 + 1) = -5 + 6 = 1
            // y = -5 + (-2 * (2 + 1)) = -5 - 6 = -11
            Assert.AreEqual(new Vector3(1f, -11f, 0f), result);
        }

        [Test]
        public void CalculateTilePosition_LargeIndices()
        {
            // Arrange
            int i = 100;
            int j = 50;
            float tileSize = 1.0f;
            float gap = 0.1f;
            Vector3 offset = Vector3.zero;

            // Act
            Vector3 result = _boardController.CalculateTilePosition(i, j, tileSize, gap, offset);

            // Assert
            // x = 100 * 1.1 = 110
            // y = -50 * 1.1 = -55
            Vector3 expected = new(110f, -55f, 0f);
            Assert.That(result.x, Is.EqualTo(expected.x).Within(0.001f));
            Assert.That(result.y, Is.EqualTo(expected.y).Within(0.001f));
            Assert.That(result.z, Is.EqualTo(expected.z).Within(0.001f));
        }
    }
}
