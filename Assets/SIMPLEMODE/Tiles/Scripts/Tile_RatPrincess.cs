using UnityEngine;
using System.Collections;
using static StringTools;
using System.Linq;
public class Tile_RatPrincess : TileInfo
{
    [SerializeField] int moneyOnRat = 1;

    public override TileInfo GetCopy()
    {
        Tile_RatPrincess newInfo = (Tile_RatPrincess)CopyBaseStatsIntoOther(new Tile_RatPrincess());
        newInfo.moneyOnRat = moneyOnRat;
        return newInfo;
    }
    public override IEnumerator OnPlacedInBoard() 
    {
        yield return base.OnPlacedInBoard();
        GameController.OnCrossed_CardEffects.AddEffect(onCrossedTile);
    }
    IEnumerator onCrossedTile(TileController tile)
    {
        if(tile != _Controller && tile._Info.tileTags.Contains(TileTags.Rat))
        {
            GameController.AddMoney(moneyOnRat);
            yield break;
        }
    }
   public override IEnumerator OnRemovedFromBoard() 
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnCrossed_CardEffects.RemoveEffect(onCrossedTile);
    }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return OnCustomMessaje("ON CROSSED ANOTHER RAT") + $" Get {moneyOnRat} coin"; }
}