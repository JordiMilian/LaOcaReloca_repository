using UnityEngine;
using System.Collections;
using static StringTools;
public class Toy_Ladybug : Toy_Info
{
   public override void OnActivatedToy()
    {
        _gameController.OnRolledDice_CardEffects.AddEffect(AddRolledValueToRandomTile);
    }
   public override void OnDeactivatedToy()
    {
        _gameController.OnRolledDice_CardEffects.RemoveEffect(AddRolledValueToRandomTile);
    }
    IEnumerator AddRolledValueToRandomTile()
    {
        int lastRolledvalue = _gameController.dicesController.LastRolledValue;

        TileController randomTile = MathJ.GetRandomTileInBoard(null, false, true);

        yield return randomTile.AddBaseDamage(lastRolledvalue);

        yield return new WaitForSeconds(0.25f);
    }
    public override string GetTooltipDescription() { return $"{OnRolledDice}: Add dices value to a random Tile"; }
}