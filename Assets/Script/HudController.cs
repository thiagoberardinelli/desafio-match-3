using System;
using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
    [Header("Score References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int pointsPerTile;
    
    [Header("Combo References")] [Space(2.5F)]
    [SerializeField] private TextMeshProUGUI comboText;

    private int _score;

    private void Start() => scoreText.text = $"Score: {_score}";
    
    public void AddScore(int numberOfTiles, int scoreMultiplier)
    {
        _score += pointsPerTile * numberOfTiles * scoreMultiplier;
        scoreText.text = $"Score: {_score}";

        if (scoreMultiplier > 1)
            comboText.text = $"Combo x{scoreMultiplier}";
    }

    public void ClearComboText()
    {
        if (comboText.text == string.Empty) return;
        comboText.text = string.Empty;
    }
}