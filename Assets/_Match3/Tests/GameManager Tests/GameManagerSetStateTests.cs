using NUnit.Framework;
using UnityEngine;

namespace GameManagerTests
{
    public class GameManagerSetStateTests
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
        public void SetState_UpdatesGameState()
        {
            _gameManager.SetState(GameState.Playing);
            Assert.AreEqual(GameState.Playing, _gameManager.GameState);

            _gameManager.SetState(GameState.Paused);
            Assert.AreEqual(GameState.Paused, _gameManager.GameState);
        }

        [Test]
        public void SetState_TriggersGameStateChangedEvent()
        {
            GameState receivedState = GameState.Start;
            bool eventFired = false;

            _gameManager.GameStateChanged += (state) =>
            {
                receivedState = state;
                eventFired = true;
            };

            _gameManager.SetState(GameState.GameOver);

            Assert.IsTrue(eventFired, "GameStateChanged event should be fired");
            Assert.AreEqual(GameState.GameOver, receivedState, "Event should pass the correct GameState");
        }
    }
}
