using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
[CreateAssetMenu(menuName = "TileProfile/DamageAdders/Pinyata", fileName = "Tile_Pinyata")]
public class Tile_Pinyata : Tile_Profile
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   

    [SerializeField] int adjacentDepth = 2;
    [SerializeField] float addedDmg, addedDmgOnlanded;
    public override string GetTooltipText() 
    {
        return$"{OnCrossed} Add {MathJ.AddDamage(addedDmg)} to tiles around in range {adjacentDepth} \n {OnLanded} Increase that amount by +{addedDmgOnlanded}";
    }

    public override IEnumerator OnPlayerLanded() 
    {
        addedDmg += addedDmgOnlanded;
        yield return base.OnPlayerLanded();
       
    }
    public override IEnumerator OnTileFinished()
    {
        List<TileController> tilesAround = MathJ.GetAdjacentTiles(_Tile, adjacentDepth);
        foreach (TileController tile in tilesAround)
        {
            CoroutineRunner.instance.StartCoroutine(tile.AddBaseDamage(addedDmg));
        }
        yield return new WaitForSeconds(MessajesManager.instance.GetDurationMultiplier());
        yield return base.OnTileFinished();
    }
}