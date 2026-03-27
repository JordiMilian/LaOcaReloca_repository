using UnityEngine;
using System.Collections;
public class Tile_TowerOfDices : TileInfo
{
   public override IEnumerator OnPlacedInBoard() 
    {
        yield return base.OnPlacedInBoard();
        GameController.OnRolledDice_CardEffects.AddEffect(OnRolledDices);
    }
   public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnRolledDice_CardEffects.RemoveEffect(OnRolledDices);
    }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnRolledDice} Deal this Tile damage x the amount of dices rolled"; }

    IEnumerator OnRolledDices()
    {
        _Controller.DamagesToDeal.Add(BaseDamage * Dices_Controller.Instance.LastRolledDicesCount);
        yield return _Controller.C_DealAllDamageToDeal();
    }
}