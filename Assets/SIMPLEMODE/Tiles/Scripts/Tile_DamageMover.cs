using System.Collections;
using UnityEngine;

public class Tile_DamageMover : Tile_Base
{
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        Tile_Base tileBehind = BoardController.TilesList[indexInBoard - 1];
        if(tileBehind is not Tile_Start)
        {
            Tile_Base tileForward = BoardController.TilesList[indexInBoard + 1];
            tileForward.AddPermaDamage(tileBehind.GetCrossedDamageAmount());
            tileBehind.AddPermaDamage(-tileBehind.GetDefaultCrossedDamage());
            yield return new WaitForSeconds(0.3f);
        }
    }

    public override string GetTooltipText()
    {
        return "On Stepped: Move the damage from the previous tile to the next tile";
    }
}
