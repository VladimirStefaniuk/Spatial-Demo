using TMPro;
using UnityEngine;

public class ScoresUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private int currentScore = 0;

    private void Start()
    {
        UpdateScoreDisplay();
    }

    public void AddScore(int value)
    {
        currentScore += value;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }
}