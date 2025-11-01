using UnityEngine;
using System.Collections;
public class Tile_RatKing : Tile_Profile
{
    [SerializeField] float ratsDamageAdder = .5f;
    [SerializeField] float addedAmountOnLanded = .5f;
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
    public override IEnumerator OnPlayerLanded() 
    { 
        yield return base.OnPlayerLanded();
        ratsDamageAdder += addedAmountOnLanded;
        
    }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override string GetTooltipText() { return $"Other Rats deal {ratsDamageAdder * 100}% more DMG \n {OnLanded} Increase that amount by {addedAmountOnLanded * 100}%"; }

    
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