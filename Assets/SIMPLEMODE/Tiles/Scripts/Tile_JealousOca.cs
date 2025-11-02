using UnityEngine;
using System.Collections;
public class Tile_JealousOca : Tile_Oca
{
    public override IEnumerator OnPlacedInBoard() 
    {
       yield return base.OnPlacedInBoard();
        GameController.OnLanded_CardEffects.AddEffect(OnLandedEffect);
    }
    IEnumerator OnLandedEffect()
    {
        TileController landedTile = BoardController.TilesList[BoardController.PlayerIndex];
        if (landedTile._Profile.tileTag == TileTags.Oca && landedTile != _Tile)
        {
            _Tile.AddBaseDamage(landedTile.GetBaseDamage());
            yield break;
        }   
    }
   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnLandedOnTag(TileTags.Oca)} Add that tile's base damage to this tile"; }

}