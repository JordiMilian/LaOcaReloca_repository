using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_IceDagger : TileInfo
{
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    public override TileInfo GetCopy()
    {
        return (Tile_IceDagger)CopyBaseStatsIntoOther(new Tile_IceDagger());
    }
}