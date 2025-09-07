using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
public class Tile_Shelter : Tile_Profile
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    public override string GetTooltipText() 
    {
        return $"{OnCrossed} For each surrounding EMPTY TILES, {MathJ.AddDamage(DealtDamagePerEmpty)}\n{OnLanded} Add {MathJ.AddDamage(PermaAddedDamage)} to each surrounding EMPTY TILES";
    }
    [SerializeField] float DealtDamagePerEmpty = 10;
    [SerializeField] float PermaAddedDamage = 20;
    public override IEnumerator OnPlayerStepped()
    {
        List<TileController> adjacentEmpties = MathJ.GetTilesAround(Tile,true);
        int emptiesCount = 0;
        foreach (TileController tile in adjacentEmpties)
        {
            if(tile._Profile.tileTag == TileTags.EmptyTile) 
            {
                emptiesCount++;
                tile.tileMovement.shakeTile(Intensity.low);
            }
        }
        Tile.DamagesToDeal.Add(DealtDamagePerEmpty * emptiesCount);
        yield return base.OnPlayerStepped();
    }
    public override IEnumerator OnPlayerLanded()
    {
        List<TileController> adjacentEmpties = MathJ.GetTilesAround(Tile,true);
        List<TileController> emptiesAround = new();

        foreach (TileController tile in adjacentEmpties)
        {
            if (tile._Profile.tileTag == TileTags.EmptyTile)
            {
                emptiesAround.Add(tile);
                tile.AddBaseDamage(PermaAddedDamage);
            }
        }

        yield return base.OnPlayerLanded();
    }
}