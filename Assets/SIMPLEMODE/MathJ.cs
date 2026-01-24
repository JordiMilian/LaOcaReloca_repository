using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public static string AddDamage(float damage) { return $"<color=blue>+{FloatToString(damage, 1)}dmg<color=black>"; }
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
    public static TileController GetRandomTileInBoard(TileController thisTile,bool ignoreSelf = true, bool ignoreStart = true,bool ignoreEnd = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;

        TileController randomTile = board.TilesList[Random.Range(0, board.TilesList.Count)];
        while(ignoreSelf && randomTile == thisTile || ignoreEnd && randomTile._Profile is Tile_End || (ignoreStart && randomTile._Profile is Tile_Start))
        {
            randomTile = board.TilesList[Random.Range(0, board.TilesList.Count)];
        }
        return randomTile;
    }
    public static int GetRandomIndexInBoard(bool ignorePlayerIndex)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        if(ignorePlayerIndex)
        {
            int random = 0;
            do
            {
                random = Random.Range(1, board.TilesList.Count - 1);
            }
            while (random == board.PlayerIndex);
            return random;
        }
        else
        {
            return Random.Range(1, board.TilesList.Count - 1);
        }
    }
    public static TileController GetRandomTileInBoardWithTag(TileTags tileTag, TileController thisTile, bool ignoreSelf = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;

        List<TileController> tilesWithTag =  GetAllTilesWithTag(tileTag, thisTile, ignoreSelf);
        if (tilesWithTag.Count == 0) { return null; }

        return tilesWithTag[Random.Range(0, tilesWithTag.Count)];
    }
    public static List<TileController> GetAllTilesWithTag(TileTags tileTag, TileController thisTile, bool ignoreSelf = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<TileController> tilesWithTag = new();
        foreach (TileController tile in board.TilesList)
        {
            if (tile._Profile.tileTags.Contains(tileTag))
            {
                if (ignoreSelf && tile == thisTile) { continue; }
                tilesWithTag.Add(tile);
            }
        }
        return tilesWithTag;
    }
    public static List<TileController> GetAdjacentTiles(TileController thisTile, int depth = 1, bool ignoreEnd = false, bool ignoreStart = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<TileController> adjacentTiles = new();
        for(int i = 0; i< depth; i++)
        {
            int positiveIndex = thisTile.indexInBoard + i + 1;
            int negativeIndex = thisTile.indexInBoard -(i + 1);
            
            if (positiveIndex < board.TilesList.Count)
            {
                TileController tile = board.TilesList[positiveIndex];
                if(tile._Profile is Tile_End && ignoreEnd) { }
                else
                {
                    adjacentTiles.Add(tile);
                }

            }
            if(negativeIndex >= 0)
            {
                TileController tile = board.TilesList[negativeIndex];
                if(tile._Profile is Tile_Start && ignoreStart) { }
                else
                {
                    adjacentTiles.Add(tile);
                }
            }
        }
        return adjacentTiles;
    }
    #region DEPRECATED
    public static List<TileController> GetTilesAround(TileController thisTile, bool ignoreSelf)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<TileController> tilesAround = new();
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
    public static List<TileController> GetVerticalAxisTiles(TileController thisTile, bool ignoreSelf = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<TileController> tilesInAxis = new List<TileController>();

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
    public static List<TileController> GetHorizontalAxisTiles(TileController thisTile, bool ignoreSelf = true)
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;
        List<TileController> tilesInAxis = new List<TileController>();

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
    public static List<TileController> GetBothAxisTiles (TileController thisTile, bool ignoreSelf = true)
    {
        List<TileController> tilesInAxis = new List<TileController>();
        tilesInAxis.AddRange(GetVerticalAxisTiles(thisTile));
        tilesInAxis.AddRange(GetHorizontalAxisTiles(thisTile));
        if (!ignoreSelf) tilesInAxis.Add(thisTile);
        return tilesInAxis;
    }
    #endregion
    #endregion
    //this functions works with Vector3 but works as If it was Vector2, meaning  (A,0,B) => (A,B). It doesnt consider Y
    public static Vector3 worldToLocal2D(Vector3 world, Vector3 pos, Vector3 right, Vector3 forward)
    {
        Vector3 posToWorld = world - pos;
        float x = Vector3.Dot(posToWorld, right);
        float z = Vector3.Dot(posToWorld, forward);
        return new Vector3(x, 0, z);
    }

    //Roll dice method


    //t es un valor de 0-1 pel que multipliquem el vector v per trobar la interseccio. Ho fem amb X i Y per separat per veure quin dels dos dona menor resultat
    //t*vx = 0.5 => t = 0.5/vx
    public static float GetSquare1Intersection(Vector3 v)
    {
        if(Mathf.Approximately(v.sqrMagnitude, 0)) { return 0; }

        float targetX = .5f * Mathf.Sign(v.x);
        float targetZ = .5f * Mathf.Sign(v.z);

        if (Mathf.Approximately(v.x, 0))
        {
            return targetZ / v.z;
        }
        if (Mathf.Approximately(v.z, 0))
        {
            return targetX / v.x;
        }

        float tx = targetX / v.x;
        float tz = targetZ / v.z;

        return Mathf.Min(tx, tz);
    }
}
