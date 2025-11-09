using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Earthquake : Tile_Profile
{
    [SerializeField] int tilesOnLanded = 3, tilesOnCrossed = 1;
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    public override IEnumerator OnPlayerLanded()
   {
        yield return base.OnPlayerLanded();
        for (int i = 0; i < tilesOnLanded; i++)
        {
            TileController randomTile = MathJ.GetRandomTileInBoard(_Tile);
            yield return randomTile.OnPlayerStepped();
            if (randomTile != null) { yield return randomTile.C_DealAllDamageToDeal(); }
        }
   }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnLanded} Trigger the OnCrossed effect of {tilesOnLanded} Random Tiles\n{OnCrossed} Trigger the OnCrossed effect of {tilesOnCrossed} Random Tiles"; }
}