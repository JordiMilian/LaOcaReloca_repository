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
        yield return GameController.Co_AddAcumulatedMultiplier(lastRolledvalue);


        Tile_Base randomTile = null;

        do
        {
            randomTile = BoardController.TilesList[Random.Range(1, BoardController.TilesList.Count)];
        }
        while (randomTile == this);


        randomTile.AddPermaDamage(lastRolledvalue);

        //Feedback
        tileMovement.shakeTile(Intensity.mid);
        yield return new WaitForSeconds(0.25f);
    }
    public override string GetTooltipText()
    {
        return $"ON DICES ROLLED: Add dices value to a random Tile";
    }
}
