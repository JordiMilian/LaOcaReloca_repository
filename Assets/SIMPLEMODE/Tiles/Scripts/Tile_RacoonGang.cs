using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static StringTools;
using NUnit.Framework;
public class Tile_RacoonGang : Tile_Profile
{
    [SerializeField] int AmountToTake = 5;
    [SerializeField] float dmgPerMoney = 3;
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped()
    { 
        yield return base.OnPlayerStepped();
        int tmpAmount = AmountToTake;
        if(GameController.GetCurrentMoney() < AmountToTake)
        {
            tmpAmount = GameController.GetCurrentMoney();
        }
        GameController.RemoveMoney(tmpAmount);

        List<TileController> tilesAround = MathJ.GetAdjacentTiles(_Tile, 1);
        foreach (TileController tile in tilesAround)
        {
            tile.AddBaseDamage(tmpAmount * dmgPerMoney);
        }
    }
   public override string GetTooltipText() { return $"{OnCrossed} Loose up to {AmountToTake} MONEY and add {AddDamage(dmgPerMoney)} per MONEY lost to Adjacent Tiles"; }
}