using System.Collections;
using UnityEngine;

public class Tile_OnCrossedAddEndTileDamage : Tile_Base
{
    [SerializeField] int damageToAdd = 5;
    [SerializeField] float multiplierOnLanded = 1.5f;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        Tile_Base endTile = BoardController.TilesList[BoardController.TilesList.Count - 1];

        endTile.AddPermaDamage(damageToAdd);
    }
    public override IEnumerator OnPlayerLanded()
    {
        Tile_Base endTile = BoardController.TilesList[BoardController.TilesList.Count - 1];
        endTile.MultiplyCrossingDamage(1.5f);
        yield return base.OnPlayerLanded();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Multiply the END TILE dmg x{multiplierOnLanded} \n{OnCrossed} Add {damageToAdd} damage to the END TILE";
    }
}
