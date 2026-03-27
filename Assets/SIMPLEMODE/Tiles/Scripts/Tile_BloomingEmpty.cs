using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TileProfile/EmptySynergy/MrBeast", fileName = "Tile_BloomingEmpty")]
public class Tile_BloomingEmpty : TileInfo
{
    [SerializeField] int addedDamageToEmptyTiles = 2;

    public override IEnumerator OnPlayerLanded()
    {
        foreach(TileController tile in BoardController.TilesList)
        {
            if(tile._Profile.tileTags.Contains(TileTags.Empty))
            {
                yield return tile.AddBaseDamage(addedDamageToEmptyTiles);
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield return base.OnPlayerLanded();
    }
    public override IEnumerator OnPlayerStepped()
    {
        TileController randomEmpty = MathJ.GetRandomTileInBoardWithTag(TileTags.Empty, _Controller, true);

        if(randomEmpty != null)
        {
            yield return randomEmpty.AddBaseDamage(addedDamageToEmptyTiles);
        }

        yield return base.OnPlayerStepped();
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Add {MathJ.AddDamage(addedDamageToEmptyTiles)} to ALL EMPTY TILES \n{OnCrossed} Add {MathJ.AddDamage(addedDamageToEmptyTiles)} to a RANDOM EMPTY TILE";
    }
}
