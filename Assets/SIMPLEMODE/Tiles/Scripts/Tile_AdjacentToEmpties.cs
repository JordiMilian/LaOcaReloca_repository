using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
public class Tile_AdjacentToEmpties : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    public override string GetTooltipText() 
    {
        return $"{ON(On.OnCrossed)} for each surrounding EMPTY TILES, {MathJ.AddMultiplier(AddedDamagePerEmpty)}";
    }
    [SerializeField] float AddedDamagePerEmpty = 10;
    public override IEnumerator OnPlayerStepped()
    {
        List<Tile_Base> adjacentEmpties = GetTilesAround(true);
        int emptiesCount = 0;
        foreach (Tile_Base tile in adjacentEmpties)
        {
            if(tile.tileTag == TileTags.EmptyTile) 
            {
                emptiesCount++;
                tile.tileMovement.shakeTile(Intensity.low);
                tile.tileMovement.DisplayMessage($"+{AddedDamagePerEmpty}", TileMessageType.AddMultiplier);
            }
        }
        yield return GameController.Co_AddAcumulatedMultiplier(AddedDamagePerEmpty * emptiesCount);
        yield return base.OnPlayerStepped();
    }
}