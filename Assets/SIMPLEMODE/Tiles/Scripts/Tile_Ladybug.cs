using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/DicesEnjoyers/Ladybug", fileName = "Tile_Ladybug")]
public class Tile_Ladybug : Tile_Profile
{
    public override IEnumerator OnPlacedInBoard()
    {
        yield return base.OnPlacedInBoard();
        GameController.OnRolledDice_CardEffects.AddEffect(AddRolledValueToRandomTile);
    }
    public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnRolledDice_CardEffects.RemoveEffect(AddRolledValueToRandomTile);
    }
    IEnumerator AddRolledValueToRandomTile()
    {
        int lastRolledvalue = GameController.dicesController.LastRolledValue;

        TileController randomTile = MathJ.GetRandomTileInBoard(_Tile, true,true);


        randomTile.AddBaseDamage(lastRolledvalue);

        //Feedback
        tileMovement.shakeTile(Intensity.mid);
        yield return new WaitForSeconds(0.25f);
    }
    public override string GetTooltipText()
    {
        return $"{OnRolledDice}: Add dices value to a random Tile";
    }
}
