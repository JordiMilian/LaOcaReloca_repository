using System.Collections;
using UnityEngine;
using static StringTools;
[CreateAssetMenu(menuName = "TileProfile/DamageMover", fileName = "Tile_DamageMover")]
public class Tile_DamageMover : TileInfo
{
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        TileController tileBehind = BoardController.TilesList[_Controller.indexInBoard - 1];
        if(tileBehind._Profile is not Tile_Start)
        {
            TileController tileForward = BoardController.TilesList[_Controller.indexInBoard + 1];
            float damageBehind = tileBehind.GetBaseDamage();
            yield return tileBehind.RemoveBaseDamage(damageBehind);
            yield return tileForward.AddBaseDamage(damageBehind);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public override string GetTooltipText()
    {
        return $"{OnCrossed} Transfer the damage from the tile behind to the tile forward";
    }
}
