using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_MrBeast : Tile_Base
{
    [SerializeField] int addedDamageToEmptyTiles = 2;

    public override IEnumerator OnPlayerLanded()
    {
        foreach(Tile_Base tile in BoardController.TilesList)
        {
            if(tile.tileTag == TileTags.EmptyTile)
            {
                tile.AddBaseDamage(addedDamageToEmptyTiles);
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield return base.OnPlayerLanded();
    }
    public override IEnumerator OnPlayerStepped()
    {
        Tile_Base randomEmpty = MathJ.GetRandomTileInBoardWithTag(TileTags.EmptyTile, this, true);

        if(randomEmpty != null)
        {
            randomEmpty.AddBaseDamage(addedDamageToEmptyTiles);
        }

        yield return base.OnPlayerStepped();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Add {MathJ.AddDamage(addedDamageToEmptyTiles)} to ALL EMPTY TILES \n{OnCrossed} Add {MathJ.AddDamage(addedDamageToEmptyTiles)} to a RANDOM EMPTY TILE";
    }
}
