using UnityEngine;
using System.Collections;
using static StringTools;
public class Toy_Compost : Toy_Profile
{
    [SerializeField] float poisonAmount = 5;
    public override void OnActivatedToy()
    {
        _gameController.OnRemovedTileFromBoard_CardEffect.AddEffect(OnRemovedCard);
    }
   public override void OnDeactivatedToy()
    {
        _gameController.OnRemovedTileFromBoard_CardEffect.RemoveEffect(OnRemovedCard);
    }
    IEnumerator OnRemovedCard()
    {
        _gameController.ApplyPoison(poisonAmount);
        yield return new WaitForSeconds(.2f);
    }
    public override string GetTooltipDescription() { return  $"{OnRemovedTileFromBoard} Apply {poisonAmount} poison"; }
}