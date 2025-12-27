using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Mushroom : Tile_Plant
{
   [SerializeField] float changeToDuplicate = .1f;
   public override IEnumerator OnPlacedInBoard() 
    {
        yield return base.OnPlacedInBoard();
        GameController.OnRolledDice_CardEffects.AddEffect(CheckForDuplicate);
    }
   public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnRolledDice_CardEffects.RemoveEffect(CheckForDuplicate);
    }
    IEnumerator CheckForDuplicate()
    {
        float randomValue = Random.Range(0f, 1f);
        if(randomValue <= changeToDuplicate)
        {
            TileController newMushroom = TilesFactory.instance.InstantiateTile(this);
            newMushroom.SetBaseDamage(baseGrowth);
            int randomIndex = Random.Range(1, BoardController.TilesList.Count - 1);
            yield return BoardController.C_AddNewTile(newMushroom, randomIndex);
        }
    }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped(); 
        yield return BoardController.C_RemoveTile(_Tile.indexInBoard); 
        GameController.remainingStepsToTake++; }
   public override string GetTooltipText() { return  base.GetTooltipText() + $",{Fragile},{NoStep}" +$"\n{OnRolledDice} {changeToDuplicate * 100}% change to spawn a copy of this tile with {baseGrowth} DMG"; }
}