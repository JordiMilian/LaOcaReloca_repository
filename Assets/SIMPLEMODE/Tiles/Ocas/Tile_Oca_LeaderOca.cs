using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/Ocas/LeaderOca", fileName = "Tile_LeaderOca")]
public class Tile_Oca_LeaderOca : Tile_Oca
{
    [SerializeField] int damagePerOca = 1;
    [SerializeField] float addedDamageAtOcas = 10;
    public override IEnumerator OnPlayerLanded()
    {
        List<TileController> ocasTiles = new();
        foreach (TileController tile in BoardController.TilesList)
        {
            if (tile._Profile.tileTag == TileTags.Oca) { ocasTiles.Add(tile); }
        }
        foreach (TileController tile in ocasTiles)
        {
            if (tile == this) { continue; }
            tile.AddBaseDamage(addedDamageAtOcas);
            tile.tileMovement.shakeTile(Intensity.low);
            yield return new WaitForSeconds(0.1f);
        }
        yield return base.OnPlayerLanded();
    }
    public override IEnumerator OnPlayerStepped()
    {
        int OcasCount = 0;
        foreach (TileController tile in BoardController.TilesList)
        {
            if (tile._Profile is Tile_Oca) { OcasCount++; }
        }
        _Tile.DamagesToDeal.Add(damagePerOca * OcasCount);
        Debug.Log($"Found {OcasCount} ocas");

        yield return base.OnPlayerStepped();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Add {MathJ.AddDamage( addedDamageAtOcas)} to all other OCA TILES \n{OnCrossed}Deal {damagePerOca} damage per OCA TILE in board";
    }
}
