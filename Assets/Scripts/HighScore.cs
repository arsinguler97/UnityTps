using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        int score = ScoreManager.Instance.GetScore();
        highScoreText.text = "Highscore: " + score;
    }
}