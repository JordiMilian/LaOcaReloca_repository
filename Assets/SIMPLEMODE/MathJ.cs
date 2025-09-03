using System.Collections.Generic;
using UnityEngine;

public static class MathJ 
{
    public static Vector2Int rotateVectorClockwise90Degrees(Vector2Int VectorToRotate)
    {
        return new Vector2Int(VectorToRotate.y, -VectorToRotate.x);
    }
    public static Vector2Int rotateVectorUnclockwise90Degrees(Vector2Int VectorToRotate)
    {
        return new Vector2Int(-VectorToRotate.y, VectorToRotate.x);
    }

    public static int SignZero(int amount) //The same as Mathf.Sign but returns a zero if given a 0 and it works with INTs
    {
        if (amount == 0) { return 0; }
        return (int)Mathf.Sign(amount);
    }

    public static string FloatToString(float value, int maxDecimals)
    {
        string result = value.ToString("F" + maxDecimals);

        if(maxDecimals == 0) { return result; }

        //remove innecessary zeros behind
        for (int i = result.Length - 1; i >= 0; i--)
        {
            char c = result[i];
            if (c == ',' || c== '.') 
            {
                if (i == result.Length - 1)
                {
                   result = result.Remove(i);
                }
                return result;
            }
            //As long as we keep finding 0, remove them, if not zero, stop
            if(c == '0')
            {
                result = result.Remove(i);
            }
            else
            {
                return result;
            }
        }
        return result;
    }
    public static string BoldText(string text)
    {
        return $"<b>{text}</b>";
    }
    public static string AddDamage(float damage) { return $"<color=blue>+{FloatToString(damage, 1)}dmg<color=black>"; }
    public static string AddMultiplier(float damage) { return $"<color=red>+{FloatToString(damage, 1)}mult<color=black>"; }
    public static int GetFibonacciValue(int n, int iterations)
    {
        int finalValue = n;
        int previousValue = n;
        int twoPreviousValue = n/2;
        for (int i = 0; i < iterations; i++)
        {
            finalValue += twoPreviousValue;
            twoPreviousValue = previousValue;
            previousValue = finalValue;
        }
        return finalValue;
    }

    #region TILES EFFECTS UTILITIES
    public static Tile_Base GetRandomTileInBoard(Tile_Base thisTile,bool ignoreSelf = true, bool ignoreStart = true,bool ignoreEnd = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;

        Tile_Base randomTile = board.TilesList[Random.Range(0, board.TilesList.Count)];
        while(ignoreSelf && randomTile == thisTile || ignoreEnd && randomTile is Tile_End || (ignoreStart && randomTile is Tile_Start))
        {
            randomTile = board.TilesList[Random.Range(0, board.TilesList.Count)];
        }
        return randomTile;
    }
    public static Tile_Base GetRandomTileInBoardWithTag(TileTags tileTag, Tile_Base thisTile, bool ignoreSelf = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;

        List<Tile_Base> tilesWithTag =  GetAllTilesWithTag(tileTag, thisTile, ignoreSelf);
        if (tilesWithTag.Count == 0) { return null; }

        return tilesWithTag[Random.Range(0, tilesWithTag.Count)];
    }
    public static List<Tile_Base> GetAllTilesWithTag(TileTags tileTag, Tile_Base thisTile, bool ignoreSelf = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<Tile_Base> tilesWithTag = new();
        foreach (Tile_Base tile in board.TilesList)
        {
            if (tile.tileTag == tileTag)
            {
                if (ignoreSelf && tile == thisTile) { continue; }
                tilesWithTag.Add(tile);
            }
        }
        return tilesWithTag;
    }
    public static List<Tile_Base> GetTilesAround(Tile_Base thisTile, bool ignoreSelf)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<Tile_Base> tilesAround = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int tileIndex = thisTile.vectorInBoard + new Vector2Int(i, j);

                if (i == 0 && j == 0)
                {
                    if (!ignoreSelf) { tilesAround.Add(board.TilesByPosition[tileIndex]); }
                    continue;
                }

                if (board.TilesByPosition.ContainsKey(tileIndex))
                {
                    tilesAround.Add(board.TilesByPosition[tileIndex]);
                }
            }
        }
        return tilesAround;
    }
    public static List<Tile_Base> GetVerticalAxisTiles(Tile_Base thisTile, bool ignoreSelf = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<Tile_Base> tilesInAxis = new List<Tile_Base>();

        Vector2Int vectorToCheck = thisTile.vectorInBoard + Vector2Int.down;
        while (board.TilesByPosition.ContainsKey(vectorToCheck))
        {
            tilesInAxis.Add(board.TilesByPosition[vectorToCheck]);
            vectorToCheck += Vector2Int.down;
        }
        vectorToCheck = thisTile.vectorInBoard + Vector2Int.up;
        while (board.TilesByPosition.ContainsKey(vectorToCheck))
        {
            tilesInAxis.Add(board.TilesByPosition[vectorToCheck]);
            vectorToCheck += Vector2Int.up;
        }
        if (!ignoreSelf) tilesInAxis.Add(thisTile);

        return tilesInAxis;
    }
    public static List<Tile_Base> GetHorizontalAxisTiles(Tile_Base thisTile, bool ignoreSelf = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<Tile_Base> tilesInAxis = new List<Tile_Base>();

        Vector2Int vectorToCheck = thisTile.vectorInBoard + Vector2Int.right;
        while (board.TilesByPosition.ContainsKey(vectorToCheck))
        {
            tilesInAxis.Add(board.TilesByPosition[vectorToCheck]);
            vectorToCheck += Vector2Int.right;
        }
        vectorToCheck = thisTile.vectorInBoard + Vector2Int.left;
        while (board.TilesByPosition.ContainsKey(vectorToCheck))
        {
            tilesInAxis.Add(board.TilesByPosition[vectorToCheck]);
            vectorToCheck += Vector2Int.left;
        }
        if (!ignoreSelf) tilesInAxis.Add(thisTile);

        return tilesInAxis;
    }
    public static List<Tile_Base> GetBothAxisTiles (Tile_Base thisTile, bool ignoreSelf = true)
    {
        List<Tile_Base> tilesInAxis = new List<Tile_Base>();
        tilesInAxis.AddRange(GetVerticalAxisTiles(thisTile));
        tilesInAxis.AddRange(GetHorizontalAxisTiles(thisTile));
        if (!ignoreSelf) tilesInAxis.Add(thisTile);
        return tilesInAxis;
    }
    #endregion
}
