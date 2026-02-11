using UnityEngine;
using System.Collections;
using static StringTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
public class Tile_BirdsWatcher : Tile_Profile
{
    [SerializeField] float dmgPerOca = 2;
    [SerializeField] float dmgOnLandedAdjacentOca = 30;

    public override IEnumerator OnPlacedInBoard()
    { 
        yield return base.OnPlacedInBoard();
        GameController.OnLanded_CardEffects.AddEffect(C_CheckForAdjacentOcas);

    }
    IEnumerator C_CheckForAdjacentOcas(TileController landedTile)
    {
        if( Mathf.Abs(_Tile.indexInBoard - landedTile.indexInBoard) == 1)
        {
            if (landedTile._Profile.tileTags.Contains(TileTags.Oca))
            {
                yield return _Tile.AddBaseDamage(dmgOnLandedAdjacentOca);
                yield break;
            }
        }
    }

    public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnLanded_CardEffects.RemoveEffect(C_CheckForAdjacentOcas);
    }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   
   public override IEnumerator OnPlayerStepped() 
    {
        float finalDamage = 0;
        List<TileController> ocas = MathJ.GetAllTilesWithTag(TileTags.Oca, _Tile, true);

        finalDamage = ocas.Count * dmgPerOca;

        /*
        List<TileController> adjacentTiles = MathJ.GetAdjacentTiles(_Tile, 1, false, true);
        foreach(TileController tile in adjacentTiles)
        {
            if (tile._Profile.tileTags.Contains(TileTags.Oca))
            {
                finalDamage += dmgPerOca;
            }
        }*/
        yield return _Tile.AddBaseDamage(finalDamage);
        yield return base.OnPlayerStepped();

    }
    public override string GetTooltipText() {
        return $"{OnCrossed} Increase {AddDamageString(dmgPerOca)} this Tile per OCA in board\n" +
             $"{OnCustomMessaje("On Landed on Adjacent OCA")} increase {AddDamageString(dmgOnLandedAdjacentOca)} this Tile";
    }
}