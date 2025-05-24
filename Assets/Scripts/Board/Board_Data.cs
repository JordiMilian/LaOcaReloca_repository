using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Board_Data 
{
    public int Height, Width;
    public List<Tile_Data> TilesList = new List<Tile_Data>();
    public int PlayerIndex { get; set; }
    public Board_Data(List<Tile_Data> data, int width, int height, int playerIndex)
    {
        Height = height;
        Width = width;
        PlayerIndex = playerIndex;
        TilesList = data;
    }
}

