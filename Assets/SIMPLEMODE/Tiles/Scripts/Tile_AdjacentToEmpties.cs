using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
public class Tile_AdjacentToEmpties : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public virtual string GetTooltipText() { }

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
                tile.AddDefaultCrossingDamage(10);
            }
        }

        yield return base.OnPlayerStepped();
    }
}