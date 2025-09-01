using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
public class Tile_AddDmgAround : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

    public override string GetTooltipText() 
    {
        return$"{OnCrossed} Add {MathJ.AddDamage(addedDmg)} to tiles around";
    }

    [SerializeField] float addedDmg;
    public override IEnumerator OnPlayerStepped()
    { 
        yield return base.OnPlayerStepped();
        List<Tile_Base> tilesAround = MathJ.GetTilesAround(this, true);
        foreach (Tile_Base tile in tilesAround)
        {
            if (tile is Tile_Start) { continue; }
            tile.AddPermaDamage(addedDmg);
        }
     
    }
}