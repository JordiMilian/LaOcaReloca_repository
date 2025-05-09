using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Board 
{
    public int Height, Width;
    public List<Tile> TilesList = new List<Tile>();
    public int PlayerIndex { get; private set; }
    public Board(List<Tile> tiles, int playerIndex, int width, int height)
    {
        if (height * width != tiles.Count) { Debug.LogError("Height or Width don't match the amount of tiles");return; }        

        Height = height;
        Width = width;
        TilesList = tiles; 
        PlayerIndex = playerIndex;
    }
    public void StepPlayer(int amount, out Tile steppedTile)
    {
        if (PlayerIndex + amount > TilesList.Count - 1) { amount = (TilesList.Count - 1) - PlayerIndex; }
        if (PlayerIndex + amount < 0) { amount = -PlayerIndex; }
        PlayerIndex = PlayerIndex + amount;
        TilesList[PlayerIndex].OnPlayerStepped();
        steppedTile = TilesList[PlayerIndex];
    }
    public void LandPlayerIn(int IndexToMove, out Tile landedTile)
    {
        landedTile = TilesList[PlayerIndex];
        Debug.Log($"Landed in:{landedTile.Index}");
        if(IndexToMove > TilesList.Count - 1) { IndexToMove = TilesList.Count - 1; }
        if(IndexToMove < 0) { IndexToMove = 0; }

        PlayerIndex = IndexToMove;
        TilesList[PlayerIndex].OnPlayerStepped();
        TilesList[PlayerIndex].OnPlayerLanded();
       
    }
    public void MovePlayerTo(int newIndex)
    {
        PlayerIndex = newIndex;
    }
    public void ReplaceTile(Tile newTile, int Index)
    {
        TilesList[Index] = newTile;
    }
}

