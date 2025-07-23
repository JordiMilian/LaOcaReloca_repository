using System.Collections;
using UnityEngine;

public class Tile_OnDiceRolledAddRandom : Tile_Base
{
    public override void OnPlacedInBoard()
    {
        base.OnPlacedInBoard();
        GameController.OnRolledDice_Event.Add(AddRolledValueToRandomTile);
    }
    public override void OnRemovedFromBoard()
    {
        base.OnRemovedFromBoard();
        GameController.OnRolledDice_Event.Remove(AddRolledValueToRandomTile);
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


        randomTile.SetDefaultCrossingDamage(randomTile.GetDefaultCrossedDamage() + lastRolledvalue);

        //Feedback
        shakeTile(Intensity.mid);
        yield return new WaitForSeconds(0.2f);
        randomTile.shakeTile(Intensity.large);
        yield return new WaitForSeconds(0.5f);
    }
    public override string GetTooltipText()
    {
        return $"ON DICES ROLLED: Add dices value to a random Tile";
    }
}
