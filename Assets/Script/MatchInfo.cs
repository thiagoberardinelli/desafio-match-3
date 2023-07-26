using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class MatchInfo
    {
        public Tile tile;
        public Vector2Int position;
        public List<List<Tile>> tileBoard;
        public List<List<bool>> matchedBoard;
        public bool horizontal;

        public MatchInfo(Tile tile, Vector2Int position, List<List<Tile>> tileBoard, List<List<bool>> matchedBoard, bool horizontal)
        {
            this.tile = tile;
            this.position = position;
            this.tileBoard = tileBoard;
            this.matchedBoard = matchedBoard;
            this.horizontal = horizontal;
        }
    }
}