using System;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : Tile
{
    public int specialType;
    
    public SpecialTile(Tile tile, int specialType)
    {
        id = tile.id;
        type = tile.type;
        this.specialType = specialType;
    }

    // Do special accordingly to tile special type
    public List<List<bool>> DoSpecial(Vector2Int position, List<List<bool>> board)
    {
        return specialType switch
        {
            0 => DoBombSpecial(position, board),
            _ => board
        };
    }

    private List<List<bool>> DoBombSpecial(Vector2Int position, List<List<bool>> board)
    {
        // Defines the limits for the sub-matrix 
        int minX = Math.Max(position.x - 1, 0);
        int maxX = Math.Min(position.x + 1, board[0].Count - 1);
        int minY = Math.Max(position.y - 1, 0);
        int maxY = Math.Min(position.y + 1, board.Count - 1);

        // Loop trough the sub-matrix and set all of its elements to true
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                board[y][x] = true;
            }
        }

        return board;
    }
}