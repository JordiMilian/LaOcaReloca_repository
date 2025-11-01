using UnityEngine;
using System.Collections;
public class Tile_RatKing : Tile_Profile
{
    public override void OnPlacedInBoard()
    { 
        base.OnPlacedInBoard(); 
        GameController.OnCrossed_CardEffects.AddEffect(OnCrossedCheck);
    }
    public override void OnRemovedFromBoard() 
    { 
        base.OnRemovedFromBoard();
        GameController.OnCrossed_CardEffects.RemoveEffect(OnCrossedCheck);
    }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override string GetTooltipText() { return $"Other Rats deal {ratsDamageAdder * 100}% more DMG"; }

    [SerializeField] float ratsDamageAdder = .5f;
    IEnumerator OnCrossedCheck()
    {
        TileController otherTile = BoardController.TilesList[BoardController.PlayerIndex];
        if (otherTile._Profile.tileTag == TileTags.Rat && otherTile != _Tile)
        {
            otherTile.DamagesToDeal.Add(otherTile.GetBaseDamage() * ratsDamageAdder);
            tileMovement.shakeTile(Intensity.low);
            yield return new WaitForSeconds(0.1f);
        }
    }
}