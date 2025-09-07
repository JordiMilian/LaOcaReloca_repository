using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
public class Tile_Pinyata : Tile_Profile
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
        List<TileController> tilesAround = MathJ.GetTilesAround(Tile, true);
        foreach (TileController tile in tilesAround)
        {
            if (tile._Profile is Tile_Start) { continue; }
            tile.AddBaseDamage(addedDmg);
        }
     
    }
}