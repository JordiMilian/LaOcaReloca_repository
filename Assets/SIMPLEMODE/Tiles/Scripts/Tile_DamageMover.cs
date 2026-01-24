using System.Collections;
using UnityEngine;
using static StringTools;
[CreateAssetMenu(menuName = "TileProfile/DamageMover", fileName = "Tile_DamageMover")]
public class Tile_DamageMover : Tile_Profile
{
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        TileController tileBehind = BoardController.TilesList[_Tile.indexInBoard - 1];
        if(tileBehind._Profile is not Tile_Start)
        {
            TileController tileForward = BoardController.TilesList[_Tile.indexInBoard + 1];
            tileForward.AddBaseDamage(tileBehind.GetBaseDamage());
            tileBehind.RemoveBaseDamage(tileBehind.GetBaseDamage());
            yield return new WaitForSeconds(0.3f);
        }
    }

    public override string GetTooltipText()
    {
        return $"{OnCrossed} Transfer the damage from the tile behind to the tile forward";
    }
}
