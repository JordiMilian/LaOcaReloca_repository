using UnityEngine;
using System.Collections;
using static StringTools;
using System.Linq;
public class Toy_Trebol : Toy_Profile
{
    [SerializeField] int moneyOnLandedOnEmpty = 2;
    public override void OnActivatedToy()
    {
        _gameController.OnLanded_CardEffects.AddEffect(OnLandedEffect);
    }
   public override void OnDeactivatedToy()
    {
        _gameController.OnLanded_CardEffects.RemoveEffect(OnLandedEffect);
    }
    IEnumerator OnLandedEffect(TileController landedTile)
    {
        if (landedTile._Profile.tileTags.Contains(TileTags.Empty))
        {
            _gameController.AddMoney(moneyOnLandedOnEmpty);
            yield return new WaitForSeconds(0.5f);
        }
    }
    public override string GetTooltipDescription() { return OnLandedOnTag(TileTags.Empty) + $" Add {moneyOnLandedOnEmpty} money"; }
}