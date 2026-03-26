using UnityEngine;
using System.Collections;
using static StringTools;
using System.Collections.Generic;
public class Tile_SuperEmpty : TileStateClass
{
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   public override IEnumerator OnPlayerLanded() 
    { 
        yield return base.OnPlayerLanded(); 
        List<TileController> emptyTiles = MathJ.GetAllTilesWithTag(TileTags.Empty,_Tile, true);

        float totalDamage = 0;
        foreach (TileController tile in emptyTiles)
        {
            totalDamage += tile.GetBaseDamage();
        }
        _Tile.DamagesToDeal.Add(totalDamage);

    }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnLanded} Deal the combined DMG of all other Empty Tiles"; }
}