using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_WalkingGiant : Tile_Profile
{
    [SerializeField] float addedDamageOnStepped = 25;
   public override IEnumerator OnPlacedInBoard()
    { 
        yield return base.OnPlacedInBoard();
        GameController.OnCrossed_CardEffects.AddEffect(OnCrossedTile);
    }
    IEnumerator OnCrossedTile()
    {
        if(Mathf.Approximately(BoardController.GetCurrentPlayerTile().GetBaseDamage(),0))
        {
            _Tile.AddBaseDamage(addedDamageOnStepped);
            tileMovement.shakeTile(Intensity.mid);
            yield return new WaitForSeconds(0.3f);
        }
    }
   public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); GameController.OnCrossed_CardEffects.RemoveEffect(OnCrossedTile); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnCustomMessaje("WHEN CROSSED A TILE WITH 0 DMG")} Increase {StringTools.AddDamage(addedDamageOnStepped)} this Tile"; }
}