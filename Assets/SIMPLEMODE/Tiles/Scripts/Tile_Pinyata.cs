using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
[CreateAssetMenu(menuName = "TileProfile/DamageAdders/Pinyata", fileName = "Tile_Pinyata")]
public class Tile_Pinyata : Tile_Profile
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   

    [SerializeField] int adjacentDepth = 2;
    [SerializeField] float addedDmg, addedDmgOnlanded;
    public override string GetTooltipText() 
    {
        return$"{OnCrossed} Add {MathJ.AddDamage(addedDmg)} to tiles around in range {adjacentDepth} \n {OnLanded} Add +{addedDmgOnlanded} more to tiles around";
    }

    
    public override IEnumerator OnPlayerStepped()
    { 
        yield return base.OnPlayerStepped();
        List<TileController> tilesAround = MathJ.GetAdjacentTiles(Tile, adjacentDepth);
        foreach (TileController tile in tilesAround)
        {
            tile.AddBaseDamage(addedDmg);
        }
    }
    public override IEnumerator OnPlayerLanded() 
    { 
        yield return base.OnPlayerLanded();
        List<TileController> tilesAround = MathJ.GetAdjacentTiles(Tile, adjacentDepth);
        foreach (TileController tile in tilesAround)
        {
            tile.AddBaseDamage(addedDmgOnlanded);
        }
        addedDmg += addedDmgOnlanded;
    }
}