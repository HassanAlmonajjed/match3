using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int moves;

    public static GameManager Instance { get; private set; }

    [field: SerializeField] public int Moves { get; set; }
    public GameState GameState { get; private set; }


    public static void ResetInstance() => Instance = null;

    public event Action<int> MovesChanged;
    public event Action<GameState> GameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetState(GameState state)
    {
        if (GameState == state) return;
        GameState = state;
        GameStateChanged?.Invoke(GameState);
    }

    public void DetectGameOver()
    {
        if (Moves > 0)
        {
            Moves--;
            MovesChanged?.Invoke(Moves);
        }

        if (Moves == 0)
        {
            SetState(GameState.GameOver);
        }
    }
}

public enum GameState
{
    Start,
    Playing,
    Paused,
    GameOver
}
