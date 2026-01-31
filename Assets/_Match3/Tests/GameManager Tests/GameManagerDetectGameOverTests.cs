using NUnit.Framework;
using UnityEngine;

namespace GameManagerTests
{
    public class GameManagerDetectGameOverTests
    {
        private GameManager _gameManager;

        [SetUp]
        public void Setup()
        {
            GameObject go = new GameObject("GameManager");
            _gameManager = go.AddComponent<GameManager>();
        }

        [TearDown]
        public void Teardown()
        {
            if (_gameManager != null)
            {
                Object.DestroyImmediate(_gameManager.gameObject);
            }
            GameManager.ResetInstance();
        }

        [Test]
        public void DetectGameOver_DecrementsMoves()
        {
            const int initialMoves = 10;
            _gameManager.Moves = initialMoves;
            
            int? receivedMoves = null;
            _gameManager.MovesChanged += (m) => receivedMoves = m;

            _gameManager.DetectGameOver();

            Assert.AreEqual(initialMoves - 1, _gameManager.Moves, "Moves should be decremented");
            Assert.AreEqual(initialMoves - 1, receivedMoves, "MovesChanged event should be fired with correct value");
        }

        [Test]
        public void DetectGameOver_ChangesStateToGameOver_WhenMovesReachedZero()
        {
            _gameManager.Moves = 1;
            
            _gameManager.DetectGameOver();

            Assert.AreEqual(0, _gameManager.Moves, "Moves should be zero");
            Assert.AreEqual(GameState.GameOver, _gameManager.GameState, "GameState should be GameOver");
        }

        [Test]
        public void DetectGameOver_DoesNotDecrement_WhenMovesAlreadyZero()
        {
            _gameManager.Moves = 0;
            
            _gameManager.DetectGameOver();

            Assert.AreEqual(0, _gameManager.Moves, "Moves should remain zero");
        }

        [Test]
        public void DetectGameOver_WhenMovesAlreadyZeroAndGameOver_DoesNotFireEvents()
        {
            // Setup: moves are 0 and state is already GameOver
            _gameManager.Moves = 0;
            _gameManager.SetState(GameState.GameOver);

            bool stateEventFired = false;
            _gameManager.GameStateChanged += (s) => stateEventFired = true;

            _gameManager.DetectGameOver();

            Assert.IsFalse(stateEventFired, "GameStateChanged should not fire if already in GameOver state");
        }
    }
}
