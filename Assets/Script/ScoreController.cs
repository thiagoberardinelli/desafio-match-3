using System;
using System.Collections;
using DG.Tweening;
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
    private Tween _comboTween;

    private void Start()
    {
        _currentThreshold = pointsToReachScore;
        scoreText.text = $"Score: {_score}";
    }

    public void AddScore(int numberOfTiles, int scoreMultiplier)
    {
        _score += pointsPerTile * numberOfTiles * scoreMultiplier;
        scoreText.text = $"Score: {_score}";

        if (scoreMultiplier <= 1) return;
        AddCombo(scoreMultiplier);
    }
    
    public bool ReachedScoreThreshold()
    {
        bool reachedThreshold = _score >= _currentThreshold;
        
        if (reachedThreshold)
            _currentThreshold = pointsToReachScore + _score;
        Debug.Log(_currentThreshold);

        return reachedThreshold;
    }

    private void AddCombo(int scoreMultiplier)
    {
        comboText.text = $"Combo x{scoreMultiplier}";
        DoComboTween(0.5F, () => {});
    }


    public void ClearComboText()
    {
        if (comboText.text == string.Empty) return;
        DoComboTween(1.5F, () => comboText.text = string.Empty);
    }

    private void DoComboTween(float duration, Action onComplete)
    {
        _comboTween?.Kill();
        _comboTween =  comboText.transform.DOShakePosition(duration, Vector3.one * 3F, 100, 15);
        _comboTween.onComplete += () => onComplete();
    }
}