using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController
{
    private List<List<Tile>> _boardTiles;
    private List<int> _tilesTypes;
    private List<int> _specialTilesTypes;
    private int _tileCount;

    public List<List<Tile>> StartGame(int boardWidth, int boardHeight)
    {
        _tilesTypes = new List<int> { 0, 1, 2, 3 };
        _specialTilesTypes = new List<int> { 0, 1 };
        _boardTiles = CreateBoard(boardWidth, boardHeight, _tilesTypes);

        CreateSpecialTile();
        
        return _boardTiles;
    }
    
    private List<List<Tile>> CreateBoard(int width, int height, List<int> tileTypes)
    {
        List<List<Tile>> board = new List<List<Tile>>(height);
        _tileCount = 0;

        for (int y = 0; y < height; y++)
        {
            board.Add(new List<Tile>(width));
            for (int x = 0; x < width; x++)
            {
                board[y].Add(new Tile { id = -1, type = -1 });
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CheckInitialMatches(tileTypes, x, y, board);
            }
        }

        return board;
    }

    private void CreateSpecialTile()
    {
        int randomY = Random.Range(0, _boardTiles.Count);
        int randomX = Random.Range(0, _boardTiles[0].Count);
        Tile tile = _boardTiles[randomY][randomX];
        int specialType = Random.Range(0, _specialTilesTypes.Count);
        _boardTiles[randomY][randomX] = new SpecialTile(tile, specialType);
    }

    private void CheckInitialMatches(List<int> tileTypes, int x, int y, List<List<Tile>> board)
    {
        List<int> noMatchTypes = new List<int>(tileTypes.Count);
        for (int i = 0; i < tileTypes.Count; i++)
        {
            noMatchTypes.Add(_tilesTypes[i]);
        }

        if (x > 1
            && board[y][x - 1].type == board[y][x - 2].type)
        {
            noMatchTypes.Remove(board[y][x - 1].type);
        }

        if (y > 1
            && board[y - 1][x].type == board[y - 2][x].type)
        {
            noMatchTypes.Remove(board[y - 1][x].type);
        }

        board[y][x].id = _tileCount++;
        board[y][x].type = noMatchTypes[Random.Range(0, noMatchTypes.Count)];
    }

    public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> newBoard = CopyBoard(_boardTiles);

        (newBoard[fromY][fromX], newBoard[toY][toX]) = (newBoard[toY][toX], newBoard[fromY][fromX]);

        for (int y = 0; y < newBoard.Count; y++)
        {
            for (int x = 0; x < newBoard[y].Count; x++)
            {
                if (x > 1
                    && newBoard[y][x].type == newBoard[y][x - 1].type
                    && newBoard[y][x - 1].type == newBoard[y][x - 2].type)
                {
                    return true;
                }
                if (y > 1
                    && newBoard[y][x].type == newBoard[y - 1][x].type
                    && newBoard[y - 1][x].type == newBoard[y - 2][x].type)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> newBoard = CopyBoard(_boardTiles);

        (newBoard[fromY][fromX], newBoard[toY][toX]) = (newBoard[toY][toX], newBoard[fromY][fromX]);

        List<BoardSequence> boardSequences = new List<BoardSequence>();
        List<List<bool>> matchedTiles;
        while (HasMatch(matchedTiles = FindMatches(newBoard)))
        {
            // Cleaning matched tiles 
            List<Vector2Int> matchedPosition = GetMatchedPositions(newBoard, matchedTiles);
            // Dropping tiles
            List<MovedTileInfo> movedTilesList = MovedTilesList(matchedPosition, newBoard);
            // Filling the board with new tiles
            List<AddedTileInfo> addedTiles = GetAddedTiles(newBoard);

            BoardSequence sequence = new BoardSequence
            {
                matchedPosition = matchedPosition,
                movedTiles = movedTilesList,
                addedTiles = addedTiles
            };
            boardSequences.Add(sequence);
        }

        _boardTiles = newBoard;
        return boardSequences;
    }

    private static List<Vector2Int> GetMatchedPositions(List<List<Tile>> newBoard, List<List<bool>> matchedTiles)
    {
        List<Vector2Int> matchedPosition = new List<Vector2Int>();
        for (int y = 0; y < newBoard.Count; y++)
        {
            for (int x = 0; x < newBoard[y].Count; x++)
            {
                if (matchedTiles[y][x])
                {
                    matchedPosition.Add(new Vector2Int(x, y));
                    newBoard[y][x] = new Tile { id = -1, type = -1 };
                }
            }
        }

        return matchedPosition;
    }
    
    private static List<MovedTileInfo> MovedTilesList(List<Vector2Int> matchedPosition, List<List<Tile>> newBoard)
    {
        Dictionary<int, MovedTileInfo> movedTiles = new Dictionary<int, MovedTileInfo>();
        List<MovedTileInfo> movedTilesList = new List<MovedTileInfo>();
        for (int i = 0; i < matchedPosition.Count; i++)
        {
            int x = matchedPosition[i].x;
            int y = matchedPosition[i].y;
            if (y > 0)
            {
                // Iterates through the lines above
                for (int j = y; j > 0; j--)
                {
                    Tile movedTile = newBoard[j - 1][x];
                    newBoard[j][x] = movedTile;
                    if (movedTile.type > -1)
                    {
                        // If the tile is already in the dict, just update its "to" position to the new one
                        if (movedTiles.ContainsKey(movedTile.id))
                        {
                            movedTiles[movedTile.id].to = new Vector2Int(x, j);
                        }
                        else
                        {
                            MovedTileInfo movedTileInfo = new MovedTileInfo
                            {
                                from = new Vector2Int(x, j - 1),
                                to = new Vector2Int(x, j)
                            };
                            movedTiles.Add(movedTile.id, movedTileInfo);
                            movedTilesList.Add(movedTileInfo);
                        }
                    }
                }

                // Adds a new "empty" tile in the first line 
                newBoard[0][x] = new Tile
                {
                    id = -1,
                    type = -1
                };
            }
        }

        return movedTilesList;
    }
    
    private List<AddedTileInfo> GetAddedTiles(List<List<Tile>> newBoard)
    {
        List<AddedTileInfo> addedTiles = new List<AddedTileInfo>();
        for (int y = newBoard.Count - 1; y > -1; y--)
        {
            for (int x = newBoard[y].Count - 1; x > -1; x--)
            {
                // Set tile properties when its empty
                if (newBoard[y][x].type == -1)
                {
                    int tileType = Random.Range(0, _tilesTypes.Count);
                    Tile tile = newBoard[y][x];
                    tile.id = _tileCount++;
                    tile.type = _tilesTypes[tileType];
                    addedTiles.Add(new AddedTileInfo
                    {
                        position = new Vector2Int(x, y),
                        type = tile.type
                    });
                }
            }
        }

        return addedTiles;
    }

    private static bool HasMatch(List<List<bool>> list)
    {
        for (int y = 0; y < list.Count; y++)
            for (int x = 0; x < list[y].Count; x++)
                if (list[y][x])
                    return true;
        return false;
    }

    private static List<List<bool>> FindMatches(List<List<Tile>> newBoard)
    {
        List<List<bool>> matchedTiles = new List<List<bool>>();
        for (int y = 0; y < newBoard.Count; y++)
        {
            matchedTiles.Add(new List<bool>(newBoard[y].Count));
            for (int x = 0; x < newBoard.Count; x++)
            {
                matchedTiles[y].Add(false);
            }
        }

        for (int y = 0; y < newBoard.Count; y++)
        {
            for (int x = 0; x < newBoard[y].Count; x++)
            {
                if (x > 1
                    && newBoard[y][x].type == newBoard[y][x - 1].type
                    && newBoard[y][x - 1].type == newBoard[y][x - 2].type)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        matchedTiles[y][x - i] = true;
                        // Updates the List<List<bool> accordingly to the SpecialTile behaviour
                        matchedTiles = IsSpecialTile(newBoard[y][x - i], new Vector2Int(x - i, y), matchedTiles, true);
                    }
                }
                if (y > 1
                    && newBoard[y][x].type == newBoard[y - 1][x].type
                    && newBoard[y - 1][x].type == newBoard[y - 2][x].type)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        matchedTiles[y - i][x] = true;
                        // Updates the List<List<bool> accordingly to the SpecialTile behaviour
                        matchedTiles = IsSpecialTile(newBoard[y - i][x], new Vector2Int(x , y - i), matchedTiles, false);
                    }
                }
            }
        }

        return matchedTiles;
    }

    private static List<List<bool>> IsSpecialTile(Tile tile, Vector2Int position, List<List<bool>> board, bool horizontalMatch)
    {
        SpecialTile specialTile = tile as SpecialTile;

        return specialTile == null ? board : specialTile.DoSpecial(position, board, horizontalMatch);
    }

    private static List<List<Tile>> CopyBoard(List<List<Tile>> boardToCopy)
    {
        List<List<Tile>> newBoard = new List<List<Tile>>(boardToCopy.Count);
        for (int y = 0; y < boardToCopy.Count; y++)
        {
            newBoard.Add(new List<Tile>(boardToCopy[y].Count));
            for (int x = 0; x < boardToCopy[y].Count; x++)
            {
                Tile tile = boardToCopy[y][x];

                if (tile is SpecialTile specialTile)
                {
                    newBoard[y].Add(new SpecialTile(specialTile, specialTile.specialType));
                }
                else
                {
                    newBoard[y].Add(new Tile { id = tile.id, type = tile.type });
                }
            }
        }

        return newBoard;
    }
}
