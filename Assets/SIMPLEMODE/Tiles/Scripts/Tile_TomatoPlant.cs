using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_TomatoPlant : Tile_Plant
{
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    [SerializeField] Tile_Profile tomatoProfile;
   public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped();
        _Tile.SetBaseDamage(0);
        TileController newTomato = TilesFactory.instance.InstantiateTile(tomatoProfile);
        newTomato.transform.position = _Tile.transform.position;
        yield return BoardController.C_AddNewTile(newTomato, MathJ.GetRandomIndexInBoard(true));
    }
   public override string GetTooltipText() { return base.GetTooltipText() + $"\n{OnCrossed} Return DMG to 0 and spawn a TOMATO"; }
}