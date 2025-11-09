using UnityEngine;
using System.Collections;
public class Tile_Collector : Tile_Profile
{
    [SerializeField] int moneyOnAddedTile = 2;
    [SerializeField] int priceOnCrossed = 2;
   public override IEnumerator OnPlacedInBoard() 
   {
        yield return base.OnPlacedInBoard();
        GameController.OnAddedNewTileToBoard_CardEffect.AddEffect(OnAddedTile);
   }
   public override IEnumerator OnRemovedFromBoard() 
   {
        yield return base.OnRemovedFromBoard();
        GameController.OnAddedNewTileToBoard_CardEffect.RemoveEffect(OnAddedTile);
    }
    IEnumerator OnAddedTile(TileController newTile)
    {
        if(newTile == _Tile) { yield break; }

        GameController.AddMoney(moneyOnAddedTile);
        tileMovement.shakeTile(Intensity.mid);
        yield break;
    }
   public override IEnumerator OnPlayerStepped() 
   {
        yield return base.OnPlayerStepped();
        GameController.RemoveMoney(priceOnCrossed);

   }
   public override string GetTooltipText() { return $"{OnAddedNewTileToBoard} Add +{moneyOnAddedTile} money \n {OnCrossed} Remove -{priceOnCrossed} money"; }
}