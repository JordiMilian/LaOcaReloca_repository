using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Crucifix : Tile_Base
{
    [SerializeField] float addedDamageToOtherTiles = 2;
    public override IEnumerator OnPlayerLanded()
    {
        
        float totalDamage = 0;

        List<Tile_Base> axisTiles = MathJ.GetBothAxisTiles(this);
        foreach (Tile_Base tile in axisTiles)
        {
            yield return addTileDamage(tile);
        }

        tileMovement.shakeTile(Intensity.large);

        DamagesToDeal.Add(totalDamage);
        yield return base.OnPlayerLanded();
        //
        IEnumerator addTileDamage(Tile_Base tile)
        {
            totalDamage += tile.GetBaseDamage();
            tile.tileMovement.shakeTile(Intensity.mid);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public override IEnumerator OnPlayerStepped()
    {
        List<Tile_Base> axisTiles = MathJ.GetBothAxisTiles(this);
        foreach (Tile_Base tile in axisTiles)
        {
            tile.AddBaseDamage(addedDamageToOtherTiles);
        }
        yield return base.OnPlayerStepped();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Deal the BASE DMG of all TILES in the same axis \n{OnCrossed} Add {MathJ.AddDamage(addedDamageToOtherTiles)} to all TILES in the same axis";
    }


}
