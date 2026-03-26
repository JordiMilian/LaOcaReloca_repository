using UnityEngine;
using System.Collections;
using static StringTools;
using System.Linq;
public class Tile_RatPrince : TileStateClass
{
    [SerializeField] float DmgOnCrossedRat = 5;
   public override IEnumerator OnPlacedInBoard() 
    {
        yield return base.OnPlacedInBoard();
        GameController.OnCrossed_CardEffects.AddEffect(onCrossedTile);
    }
    IEnumerator onCrossedTile(TileController tile)
    {
        if(tile != _Tile && tile._Profile.tileTags.Contains(TileTags.Rat))
        {
            yield return _Tile.AddBaseDamage(DmgOnCrossedRat);
        }
    }
   public override IEnumerator OnRemovedFromBoard() 
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnCrossed_CardEffects.RemoveEffect(onCrossedTile);
    }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() {
        return OnCustomMessaje("ON CROSSED ANOTHER RAT") + $" Increase this tile {AddDamageString(DmgOnCrossedRat)}";
            }
}