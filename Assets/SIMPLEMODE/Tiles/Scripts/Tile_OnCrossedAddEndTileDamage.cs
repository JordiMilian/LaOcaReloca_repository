using System.Collections;
using UnityEngine;

public class Tile_OnCrossedAddEndTileDamage : Tile_Profile
{
    [SerializeField] int damageToAdd = 5;
    [SerializeField] float multiplierOnLanded = 1.5f;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        TileController endTile = BoardController.TilesList[BoardController.TilesList.Count - 1];

        endTile.AddBaseDamage(damageToAdd);
    }
    public override IEnumerator OnPlayerLanded()
    {
        TileController endTile = BoardController.TilesList[BoardController.TilesList.Count - 1];
        endTile.MultiplyBaseDamage(1.5f);
        yield return base.OnPlayerLanded();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Multiply the END TILE dmg x{multiplierOnLanded} \n{OnCrossed} Add {damageToAdd} damage to the END TILE";
    }
}
