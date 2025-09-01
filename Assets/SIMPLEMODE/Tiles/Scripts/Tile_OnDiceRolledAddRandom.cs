using System.Collections;
using UnityEngine;

public class Tile_OnDiceRolledAddRandom : Tile_Base
{
    public override void OnPlacedInBoard()
    {
        base.OnPlacedInBoard();
        GameController.OnRolledDice_CardEffects.AddEffect(AddRolledValueToRandomTile);
    }
    public override void OnRemovedFromBoard()
    {
        base.OnRemovedFromBoard();
        GameController.OnRolledDice_CardEffects.RemoveEffect(AddRolledValueToRandomTile);
    }
    IEnumerator AddRolledValueToRandomTile()
    {
        int lastRolledvalue = GameController.dicesController.LastRolledValue;

        Tile_Base randomTile = null;

        do
        {
            randomTile = BoardController.TilesList[Random.Range(1, BoardController.TilesList.Count)];
        }
        while (randomTile == this);


        randomTile.AddPermaDamage(lastRolledvalue * 2);

        //Feedback
        tileMovement.shakeTile(Intensity.mid);
        yield return new WaitForSeconds(0.25f);
    }
    public override string GetTooltipText()
    {
        return $"{ON(OnEnum.OnRolledDice)}: Add dices value x2 to a random Tile";
    }
}
