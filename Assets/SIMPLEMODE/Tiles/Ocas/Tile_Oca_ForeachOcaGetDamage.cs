using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Oca_ForeachOcaGetDamage : Tile_Oca
{
    [SerializeField] int damagePerOca = 1;
    [SerializeField] float addedDamageAtOcas = 10;
    public override IEnumerator OnPlayerLanded()
    {
        List<Tile_Base> ocasTiles = new();
        foreach (Tile_Base tile in BoardController.TilesList)
        {
            if (tile.tileTag == TileTags.Oca) { ocasTiles.Add(tile); }
        }
        foreach (Tile_Base tile in ocasTiles)
        {
            if (tile == this) { continue; }
            tile.AddPermaDamage(addedDamageAtOcas);
            tile.tileMovement.shakeTile(Intensity.low);
            yield return new WaitForSeconds(0.1f);
        }
        yield return basePlayerLanded();
    }
    public override IEnumerator OnPlayerStepped()
    {
        int OcasCount = 0;
        foreach (Tile_Base tile in BoardController.TilesList)
        {
            if (tile is Tile_Oca) { OcasCount++; }
        }
        yield return GameController.Co_AddAcumulatedDamage(damagePerOca * OcasCount);
        Debug.Log($"Found {OcasCount} ocas");

        yield return base.OnPlayerStepped();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Add {MathJ.AddDamage( addedDamageAtOcas)} to all other OCA TILES \n{OnCrossed}Deal {damagePerOca} damage per OCA TILE in board";
    }
}
