using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameHandler : MonoBehaviour
{
    private GameController gameController;

    [SerializeField] private int boardWidth = 10;
    [SerializeField] private int boardHeight = 10;
    [SerializeField] private BoardView boardView;
    [SerializeField] private ScoreController hudController;

    private int selectedX, selectedY = -1;
    private bool isAnimating;

    private void Awake()
    {
        gameController = new GameController();
        boardView.OnTileClick += OnTileClick;
    }

    private void Start()
    {
        List<List<Tile>> board = gameController.StartGame(boardWidth, boardHeight);
        boardView.CreateBoard(board);
    }

    private void OnTileClick(int x, int y)
    {
        if (isAnimating) return;

        if (selectedX > -1 && selectedY > -1)
        {
            if (Mathf.Abs(selectedX - x) + Mathf.Abs(selectedY - y) > 1)
            {
                selectedX = -1;
                selectedY = -1;
            }
            else
            {
                isAnimating = true;
                boardView.SwapTiles(selectedX, selectedY, x, y).onComplete += () =>
                {
                    bool isValid = gameController.IsValidMovement(selectedX, selectedY, x, y);
                    if (!isValid)
                    {
                        boardView.SwapTiles(x, y, selectedX, selectedY)
                        .onComplete += () => isAnimating = false;
                    }
                    else
                    {
                        List<BoardSequence> swapResult = gameController.SwapTile(selectedX, selectedY, x, y);
                        AnimateBoard(swapResult, 0, () =>
                        {
                            hudController.ClearComboText();
                            isAnimating = false;
                        });
                    }

                    selectedX = -1;
                    selectedY = -1;
                };
            }
        }
        else
        {
            selectedX = x;
            selectedY = y;
        }
    }

    private void AnimateBoard(List<BoardSequence> boardSequences, int sequenceIndex, Action onComplete)
    {
        Sequence sequence = DOTween.Sequence();

        BoardSequence boardSequence = boardSequences[sequenceIndex];
        sequence.Append(boardView.DestroyTiles(boardSequence.matchedPosition));
        sequence.Append(boardView.MoveTiles(boardSequence.movedTiles));
        sequence.Append(boardView.CreateTile(boardSequence.addedTiles));

        sequenceIndex++;
        hudController.AddScore(boardSequence.matchedPosition.Count, sequenceIndex);
        if (sequenceIndex< boardSequences.Count)
        {
            sequence.onComplete += () => AnimateBoard(boardSequences, sequenceIndex, onComplete);
        }
        else
        {
            sequence.onComplete += () => onComplete();
        }
    }
}
