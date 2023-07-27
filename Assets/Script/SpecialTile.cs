using System;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class SpecialTile : Tile
{
    public readonly int specialType;

    public SpecialTile(Tile tile, int specialType)
    {
        id = tile.id;
        type = tile.type;
        this.specialType = specialType;
    }

    // Do special accordingly to tile special type
    public List<List<bool>> DoSpecial(MatchInfo matchInfo)
    {
        // // For test purposes:
        // return DoColorCleanerSpecial(matchInfo);
        
        return specialType switch
        {
            0 => DoBombSpecial(matchInfo),
            1 => DoLineBreakerSpecial(matchInfo),
            2 => DoColorCleanerSpecial(matchInfo),
            _ => matchInfo.matchedBoard
        };
    }

    private List<List<bool>> DoBombSpecial(MatchInfo matchInfo)
    {
        Vector2Int matchPosition = matchInfo.position;
        List<List<bool>> matchedBoard = matchInfo.matchedBoard;

        // Defines the limits for the sub-matrix 
        int minX = Math.Max(matchPosition.x - 1, 0);
        int maxX = Math.Min(matchPosition.x + 1, matchInfo.matchedBoard[0].Count - 1);
        int minY = Math.Max(matchPosition.y - 1, 0);
        int maxY = Math.Min(matchPosition.y + 1, matchInfo.matchedBoard.Count - 1);

        // Loop trough the sub-matrix and set all of its elements to true
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                matchedBoard[y][x] = true;
            }
        }

        return matchedBoard;
    }

    private List<List<bool>> DoLineBreakerSpecial(MatchInfo matchInfo)
    {
        Vector2Int matchPosition = matchInfo.position;
        List<List<bool>> matchedBoard = matchInfo.matchedBoard;
        
        // If horizontal match set all tiles in the match line to true
        if (matchInfo.horizontal)
        {
            for (int i = 0; i < matchInfo.matchedBoard.Count; i++)
            {
                matchedBoard[matchPosition.y][i] = true;
            }
        }
        // Else, set all tiles int the match column to true
        else
        {
            for (int i = 0; i < matchInfo.matchedBoard[0].Count; i++) 
                matchedBoard[i][matchPosition.x] = true;
        }

        return matchedBoard;
    }

    private List<List<bool>> DoColorCleanerSpecial(MatchInfo matchInfo)
    {
        Tile matchedTile = matchInfo.tile;
        List<List<bool>> matchedBoard = matchInfo.matchedBoard;
        List<List<Tile>> tileBoard = matchInfo.tileBoard;
        
        // Loops through board elements and checks if its type is the same of the matched tile
        for (int y = 0; y < matchedBoard.Count; y++)
        {
            for (int x = 0; x < matchedBoard[y].Count; x++)
            {
                if (tileBoard[y][x].type != matchedTile.type) continue;
                matchedBoard[y][x] = true;
            }
        }

        return matchedBoard;
    }
}