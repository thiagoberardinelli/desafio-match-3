using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("Score References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int pointsPerTile;
    
    [Header("Combo References")] [Space(2.5F)]
    [SerializeField] private TextMeshProUGUI comboText;

    public static int score;

    private void Start() => scoreText.text = $"Score: {score}";
    
    public void AddScore(int numberOfTiles, int scoreMultiplier)
    {
        score += pointsPerTile * numberOfTiles * scoreMultiplier;
        scoreText.text = $"Score: {score}";

        if (scoreMultiplier > 1)
            comboText.text = $"Combo x{scoreMultiplier}";
    }

    public void ClearComboText()
    {
        if (comboText.text == string.Empty) return;
        comboText.text = string.Empty;
    }
}