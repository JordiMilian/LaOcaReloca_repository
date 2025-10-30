using UnityEngine;
using System.Collections;
public class Tile_TowerOfDices : Tile_Profile
{
   public override void OnPlacedInBoard() 
    {
        base.OnPlacedInBoard();
        GameController.OnRolledDice_CardEffects.AddEffect(OnRolledDices);
    }
   public override void OnRemovedFromBoard()
    {
        base.OnRemovedFromBoard();
        GameController.OnRolledDice_CardEffects.RemoveEffect(OnRolledDices);
    }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnRolledDice} Deal this Tile damage x the amount of dices rolled"; }

    IEnumerator OnRolledDices()
    {
        _Tile.DamagesToDeal.Add(BaseDamage * Dices_Controller.Instance.LastRolledDicesCount);
        yield return _Tile.C_DealAllDamageToDeal();
    }
}