using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Reflexion : TileInfo
{
   //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped()
   { 
        yield return base.OnPlayerStepped();
        TileController tileBehind = BoardController.TilesList[_Controller.indexInBoard - 1];
        if(tileBehind._Info is not Tile_Start)
        {
            TileController newTile =  TilesFactory.instance.InstantiateTileCopy(tileBehind._Info);
            newTile.SetBaseDamage(0);
            newTile.transform.position = tileBehind.transform.position;

            yield return BoardController.C_AddNewTile(newTile, tileBehind.indexInBoard);
        }
    }
    public override string GetTooltipText() { return $"{OnCrossed} Create a copy of the Tile behind with DMG 0"; }
}