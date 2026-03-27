using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/EndSynergy/Investor", fileName = "Tile_Investor")]
public class Tile_OnCrossedAddEndTileDamage : TileInfo
{
    [SerializeField] int damageToAdd = 5;
    [SerializeField] float multiplierOnLanded = 1.5f;
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        TileController endTile = BoardController.TilesList[BoardController.TilesList.Count - 1];

        yield return endTile.AddBaseDamage(damageToAdd);
    }
    public override IEnumerator OnPlayerLanded()
    {
        TileController endTile = BoardController.TilesList[BoardController.TilesList.Count - 1];
        yield return  endTile.C_MultiplyBaseDamage(multiplierOnLanded);
        yield return base.OnPlayerLanded();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Multiply the END TILE dmg x{multiplierOnLanded} \n{OnCrossed} Give {StringTools.AddDamageString(damageToAdd)} to the END TILE";
    }
}
