using System.Collections;
using UnityEngine;

public class Tile_OnCrossedAddEndTileDamage : Tile_Base
{
    [SerializeField] int damageToAdd = 5;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        Tile_Base endTile = BoardController.TilesList[BoardController.TilesList.Count - 1];

        endTile.SetDefaultCrossingDamage(endTile.GetDefaultCrossedDamage() + damageToAdd);
    }
    public override string GetTooltipText()
    {
        return $"On Crossed: Add {damageToAdd} damage to the end tile";
    }
}
