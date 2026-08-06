using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Mushroom : Tile_Plant
{
   [SerializeField] float changeToDuplicate = .1f;
    public override TileInfo GetCopy()
    {
        Tile_Mushroom newInfo = (Tile_Mushroom)CopyBaseStatsIntoOther(new  Tile_Mushroom());
        newInfo.baseGrowth = baseGrowth;
        newInfo.changeToDuplicate = changeToDuplicate;
        return newInfo;
    }
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
            TileController newMushroom = TilesFactory.instance.InstantiateTileCopy(this);
            newMushroom.SetBaseDamage(baseGrowth);
            int randomIndex = MathJ.GetRandomIndexInBoard(true);
            yield return BoardController.C_AddNewTile(newMushroom, randomIndex);
        }
    }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

   public override string GetTooltipText() { return base.GetTooltipText()+ $"\n{OnRolledDice} {changeToDuplicate * 100}% change to spawn a copy of this tile with {baseGrowth} DMG"; }
}