using UnityEngine;
using System.Collections;
using static StringTools;
public class Toy_Compost : Toy_Info
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
    public override Toy_Info GetCopy()
    {
        Toy_Compost newInfo = (Toy_Compost)CopyBaseStatsIntoOther(new Toy_Compost());
        newInfo.poisonAmount = poisonAmount;
        return newInfo;
    }
    public override string GetTooltipDescription() { return  $"{OnRemovedTileFromBoard} Apply {poisonAmount} poison"; }
}