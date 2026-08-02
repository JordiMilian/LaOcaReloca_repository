using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
[CreateAssetMenu(menuName = "TileProfile/DamageAdders/Pinyata", fileName = "Tile_Pinyata")]
public class Tile_Pinyata : TileInfo
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   

    [SerializeField] int adjacentDepth = 2;
    [SerializeField] float addedDmg, addedDmgOnlanded;

    public override TileInfo GetCopy()
    {
        Tile_Pinyata newInfo = (Tile_Pinyata)CopyBaseStatsIntoOther(new Tile_Pinyata());
        newInfo.adjacentDepth = adjacentDepth;
        newInfo.addedDmg = addedDmg;
        newInfo.addedDmgOnlanded = addedDmgOnlanded;
        return newInfo;
    }
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
        List<TileController> tilesAround = MathJ.GetAdjacentTiles(_Controller, adjacentDepth);
        foreach (TileController tile in tilesAround)
        {
            CoroutineRunner.instance.StartCoroutine(tile.AddBaseDamage(addedDmg));
        }
        yield return new WaitForSeconds(MessajesManager.instance.GetDurationMultiplier());
        yield return base.OnTileFinished();
    }
}