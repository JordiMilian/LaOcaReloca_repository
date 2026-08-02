using UnityEngine;
using System.Collections;
using System.Linq;
public class Tile_Trebol : TileInfo
{
    [SerializeField] int moneyOnLandedOnEmpty = 6;
    public override IEnumerator OnPlacedInBoard() 
   {
        yield return base.OnPlacedInBoard();
        GameController.OnLanded_CardEffects.AddEffect(OnLandedEffect);
    }
    IEnumerator OnLandedEffect(TileController landedTile)
    {
        if (landedTile._Info.tileTags.Contains(TileTags.Empty))
        {
            GameController.AddMoney(moneyOnLandedOnEmpty);
            tileMovement.shakeTile(Intensity.mid);
            yield return new WaitForSeconds(0.5f);
        }
    }
   public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); GameController.OnLanded_CardEffects.RemoveEffect(OnLandedEffect); }

   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() 
    { 
       return OnLandedOnTag(TileTags.Empty) + $" Add {moneyOnLandedOnEmpty} money";
    }
    public override TileInfo GetCopy()
    {
        Tile_Trebol newInfo = (Tile_Trebol)CopyBaseStatsIntoOther(new Tile_Trebol());
        newInfo.moneyOnLandedOnEmpty = moneyOnLandedOnEmpty;
        return newInfo;
    }
}