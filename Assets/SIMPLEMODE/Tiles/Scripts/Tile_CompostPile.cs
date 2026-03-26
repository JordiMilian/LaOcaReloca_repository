using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_CompostPile : TileStateClass
{
    [SerializeField] float poisonAmount = 20;
    public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); GameController.OnRemovedTileFromBoard_CardEffect.AddEffect(OnRemovedCard); }
   public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); GameController.OnRemovedTileFromBoard_CardEffect.RemoveEffect(OnRemovedCard); }

    IEnumerator OnRemovedCard()
    {
        GameController.ApplyPoison(poisonAmount);
        tileMovement.shakeTile(Intensity.mid);
        yield return new WaitForSeconds(.2f);
    }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnRemovedTileFromBoard} Apply {poisonAmount} poison"; }
}