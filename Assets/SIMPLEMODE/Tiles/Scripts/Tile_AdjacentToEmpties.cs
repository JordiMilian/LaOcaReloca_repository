using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
public class Tile_AdjacentToEmpties : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    public override string GetTooltipText() 
    {
        return $"{ON(On.OnCrossed)} if surrounded by EMPTY TILES, add {AddedDamageAround} to all of them";
    }
    [SerializeField] float AddedDamageAround = 10;
    public override IEnumerator OnPlayerStepped()
    {
        bool isSurrounded = true;
        List<Tile_Base> adjacentEmpties = GetTilesAround(true);
        foreach(Tile_Base tile in adjacentEmpties)
        {
            if(tile.tileTag != TileTags.EmptyTile) 
            {
                isSurrounded = false;
                break;
            }
        }
        if(isSurrounded)
        {
            foreach (Tile_Base tile in adjacentEmpties)
            {
                tile.AddDefaultCrossingDamage(AddedDamageAround);
            }
        }

        yield return base.OnPlayerStepped();
    }
}