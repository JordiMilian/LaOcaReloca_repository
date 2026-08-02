using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_SpiderToken : TileInfo
{
    [SerializeField] float poison = 10;
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

    public override TileInfo GetCopy()
    {
        Tile_SpiderToken newInfo = (Tile_SpiderToken)CopyBaseStatsIntoOther(new Tile_SpiderToken());
        newInfo.poison = poison;
        return newInfo;
    }
    public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped();
        GameController.ApplyPoison(poison);
    }
   public override string GetTooltipText() { return base.GetTooltipText()+$"{OnCrossed} Apply {poison} poison"; }
}