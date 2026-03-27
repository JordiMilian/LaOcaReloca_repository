using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_PoisonSpikes : TileInfo
{
    [SerializeField] int poisonOnCrossed = 15;
    [SerializeField] int addedPoisonOnLanded = 5;
   //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   public override IEnumerator OnPlayerLanded() 
    { 
        yield return base.OnPlayerLanded();
        poisonOnCrossed += addedPoisonOnLanded;
    }
    public override IEnumerator OnTileFinished()
    {
        GameController.ApplyPoison(poisonOnCrossed);
        yield return base.OnTileFinished();
    }
    public override string GetTooltipText() { return $"{OnCrossed} Deal {poisonOnCrossed} poison" +
            $"\n{OnLanded} Increase that amount by {addedPoisonOnLanded}"; }
}