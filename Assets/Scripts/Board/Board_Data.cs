using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Board_Data 
{
    public int Height, Width;
    public List<Tile_Data> TilesList = new List<Tile_Data>();
    public int PlayerIndex { get; set; }
    public Board_Data(List<Tile_Data> tiles, int playerIndex, int width, int height)
    {
        if (height * width != tiles.Count) { Debug.LogError("Height or Width don't match the amount of tiles");return; }        

        Height = height;
        Width = width;
        TilesList = tiles; 
        PlayerIndex = playerIndex;
    }

}

