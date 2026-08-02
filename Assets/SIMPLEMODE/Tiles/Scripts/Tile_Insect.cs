using UnityEngine;
using System.Collections;
public class Tile_Insect : TileInfo
{
   public override IEnumerator OnPlacedInBoard()
    {
        base.OnPlacedInBoard();
        GameController.OnInsectFly.AddEffect(Fly);
        GameController.OnInsectsMoved_CardEffects.AddEffect(OnLandedFly);
        yield break;
    }
   public override IEnumerator OnRemovedFromBoard() 
    {
        base.OnRemovedFromBoard();
        GameController.OnInsectFly.RemoveEffect(Fly);
        GameController.OnInsectsMoved_CardEffects.RemoveEffect(OnLandedFly);
        yield break;
    }

    public virtual IEnumerator Fly()
    {
        int RandomIndex = MathJ.GetRandomIndexInBoard(true);
        BoardController.MoveTileInBoard(_Controller.indexInBoard, RandomIndex);
        yield break;
    }
    public virtual IEnumerator OnLandedFly()
    {
        yield break;
    }
   public override IEnumerator OnPlayerLanded() //Insects should make base.OnPlayerLanded by the end of its logic
    {
        yield return base.OnPlayerLanded(); 
        yield return BoardController.C_RemoveTile(_Controller.indexInBoard);
    }

    public override TileInfo GetCopy()
    {
        Tile_Insect newInfo = (Tile_Insect)CopyBaseStatsIntoOther(new Tile_Insect());
        return newInfo;
    }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    //public override string GetTooltipText() { }
}