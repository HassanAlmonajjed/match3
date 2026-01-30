using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UITests
{
    [TestFixture]
    public class FillBarControllerTests
    {
        [Test]
        [TestCase(0.0f, 0)]
        [TestCase(0.29f, 0)]
        [TestCase(0.3f, 1)]
        [TestCase(0.59f, 1)]
        [TestCase(0.6f, 2)]
        [TestCase(0.89f, 2)]
        [TestCase(0.9f, 3)]
        [TestCase(1.0f, 3)]
        public void CalculateFilledStars_ThreeStars_ReturnsCorrectCount(float progress, int expectedCount)
        {
            // Act
            int result = FillBarController.CalculateFilledStars(progress, 3);

            // Assert
            Assert.AreEqual(expectedCount, result, $"Failed for progress: {progress}");
        }

        [Test]
        [TestCase(0.0f, 0)]
        [TestCase(0.44f, 0)] // threshold 1/2 * 0.9 = 0.45
        [TestCase(0.45f, 1)]
        [TestCase(0.89f, 1)] // threshold 2/2 * 0.9 = 0.9
        [TestCase(0.9f, 2)]
        [TestCase(1.0f, 2)]
        public void CalculateFilledStars_TwoStars_ReturnsCorrectCount(float progress, int expectedCount)
        {
            // Act
            int result = FillBarController.CalculateFilledStars(progress, 2);

            // Assert
            Assert.AreEqual(expectedCount, result, $"Failed for progress: {progress}");
        }

        [Test]
        [TestCase(0.0f, 0)]
        [TestCase(0.17f, 0)] // 1/5 * 0.9 = 0.18
        [TestCase(0.18f, 1)]
        [TestCase(0.35f, 1)] // 2/5 * 0.9 = 0.36
        [TestCase(0.36f, 2)]
        [TestCase(0.53f, 2)] // 3/5 * 0.9 = 0.54
        [TestCase(0.54f, 3)]
        [TestCase(0.71f, 3)] // 4/5 * 0.9 = 0.72
        [TestCase(0.72f, 4)]
        [TestCase(0.89f, 4)] // 5/5 * 0.9 = 0.9
        [TestCase(0.9f, 5)]
        [TestCase(1.0f, 5)]
        public void CalculateFilledStars_FiveStars_ReturnsCorrectCount(float progress, int expectedCount)
        {
            // Act
            int result = FillBarController.CalculateFilledStars(progress, 5);

            // Assert
            Assert.AreEqual(expectedCount, result, $"Failed for progress: {progress}");
        }

        [Test]
        public void CalculateFilledStars_ZeroStars_ReturnsZero()
        {
            // Act
            int result = FillBarController.CalculateFilledStars(0.5f, 0);

            // Assert
            Assert.AreEqual(0, result);
        }
    }
}
