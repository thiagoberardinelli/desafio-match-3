using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("Score References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int pointsPerTile;
    [SerializeField] private int pointsToReachScore;

    [Header("Combo References")] [Space(2.5F)]
    [SerializeField] private TextMeshProUGUI comboText;

    private int _score;
    private int _currentThreshold;

    private void Start()
    {
        _currentThreshold = pointsToReachScore;
        scoreText.text = $"Score: {_score}";
    }

    public void AddScore(int numberOfTiles, int scoreMultiplier)
    {
        _score += pointsPerTile * numberOfTiles * scoreMultiplier;
        scoreText.text = $"Score: {_score}";

        if (scoreMultiplier > 1)
            comboText.text = $"Combo x{scoreMultiplier}";
    }

    public bool ReachedScoreThreshold()
    {
        bool reachedThreshold = _score >= _currentThreshold;
        _currentThreshold = pointsToReachScore + _score;
        Debug.Log(_currentThreshold);

        return reachedThreshold;
    }

    public void ClearComboText()
    {
        if (comboText.text == string.Empty) return;
        comboText.text = string.Empty;
    }
}