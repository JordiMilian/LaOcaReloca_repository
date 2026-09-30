using UnityEngine;
using System.Collections;
using static StringTools;
using System.Linq;
public class Tile_RatPrince : TileInfo
{
    [SerializeField] float DmgOnCrossedRat = 5;
   public override void EnableExtraLogic()
    {
        GameController.OnCrossed_CardEffects.AddEffect(onCrossedTile);
    }
    IEnumerator onCrossedTile(TileController tile)
    {
        if(tile != _Controller && tile._Info.tileTags.Contains(TileTags.Rat))
        {
            yield return _Controller.AddBaseDamage(DmgOnCrossedRat);
        }
    }
   public override void DisableExtraLogic()
    {
        GameController.OnCrossed_CardEffects.RemoveEffect(onCrossedTile);
    }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() {
        return OnCustomMessaje("ON CROSSED ANOTHER RAT") + $" Increase this tile {AddDamageString(DmgOnCrossedRat)}";
            }
}