using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    public float MusicVolume { get; set; } = 1f;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    private void Start()
    {
        CurrentScore = 0;
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;

        if (CurrentScore > HighScore)
            HighScore = CurrentScore;
    }

    public void ResetGameState()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CurrentScore = 0;
    }
}