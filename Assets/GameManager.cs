using UnityEngine;

public enum GameState { Start, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentState;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        switch (newState)
        {
            case GameState.Start:             
                break;
            case GameState.Playing:               
                break;
            case GameState.GameOver:
                break;
        }
    }
}
