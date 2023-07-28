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
    
    [Header("Controllers")]
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private AudioController audioController;

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
                        List<BoardSequence> swapResult = gameController.SwapTile(selectedX, selectedY, x, y, out Action<BoardView> createSpecialTile);
                        
                        AnimateBoard(swapResult, 0, () =>
                        {
                            scoreController.ClearComboText();
                            isAnimating = false;
                            
                            // If after the sequence is done, the new special tile is reached invoke the action
                            if (scoreController.ReachedScoreThreshold())
                                createSpecialTile.Invoke(boardView);
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
        scoreController.AddScore(boardSequence.matchedPosition.Count, sequenceIndex);
        audioController.PlaySound("Match_SFX", sequenceIndex);
        
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
