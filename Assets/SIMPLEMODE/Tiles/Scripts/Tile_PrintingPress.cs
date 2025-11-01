using UnityEngine;
using System.Collections;
public class Tile_PrintingPress : Tile_Profile
{
   //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped()
   { 
        yield return base.OnPlayerStepped();
        TileController tileBehind = BoardController.TilesList[_Tile.indexInBoard - 1];
        if(tileBehind._Profile is not Tile_Start)
        {
            TileController newTile =  TilesFactory.instance.InstantiateTile(tileBehind._Profile);
            newTile.SetBaseDamage(0);

            BoardController.AddNewTile(newTile, _Tile.indexInBoard);
        }
        BoardController.RemoveTile(_Tile.indexInBoard);
    }
    public override string GetTooltipText() { return $"{OnCrossed} Create a copy of the Tile behind with DMG 0 and destroy itself"; }
}