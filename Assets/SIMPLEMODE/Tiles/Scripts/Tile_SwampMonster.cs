using UnityEngine;
using System.Collections;
public class Tile_SwampMonster : Tile_Profile
{
    [SerializeField] Tile_Profile swampTokenProfile;
    [SerializeField] float DamageAddedOnCross = 100;
    public override IEnumerator OnPlacedInBoard() 
   {
        yield return base.OnPlacedInBoard();
        TileController newSwamp01 = TilesFactory.instance.InstantiateTile(swampTokenProfile);
        newSwamp01.transform.position = _Tile.transform.position;

        yield return BoardController.C_AddNewTile(newSwamp01, _Tile.indexInBoard);

        TileController newSwamp02 = TilesFactory.instance.InstantiateTile(swampTokenProfile);
        newSwamp02.transform.position = _Tile.transform.position;

        yield return BoardController.C_AddNewTile(newSwamp02, _Tile.indexInBoard +1);
    }
   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() 
   { 
        _Tile.AddBaseDamage(DamageAddedOnCross);
        yield return base.OnPlayerStepped();
    }
   public override string GetTooltipText() { return $"{OnEnterInBoard} Spawn 2 SWAMP tokens around \n {OnCrossed} Add {MathJ.AddDamage(DamageAddedOnCross)} to this tile"; }
}