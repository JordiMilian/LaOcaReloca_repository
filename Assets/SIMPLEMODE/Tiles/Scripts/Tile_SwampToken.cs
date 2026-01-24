using UnityEngine;
using System.Collections;
public class Tile_SwampToken : Tile_Profile
{
    /*
    public override IEnumerator OnPlacedInBoard() 
    { 
        yield return base.OnPlacedInBoard();
        GameController.OnRolledDice_CardEffects.AddEffect(onRolledDice);
    }
    public override IEnumerator OnRemovedFromBoard() 
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnRolledDice_CardEffects.RemoveEffect(onRolledDice);
    }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

    public int remainingSteps = 2;
    
    IEnumerator onRolledDice()
    {
        if(BoardController.GetCurrentPlayerTile() == _Tile)
        {
            if(remainingSteps > 0 && GameController.remainingStepsToTake > 0)
            {
                remainingSteps--;
                GameController.remainingStepsToTake--;
                yield return BoardController.V_JumpPlayerToNewPos();
            }
        }
    }
   public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped();

        remainingSteps = 2;
        remainingSteps--;
        if(GameController.remainingStepsToTake > 1)
        {
            remainingSteps--;
            GameController.remainingStepsToTake--;
            yield return BoardController.V_JumpPlayerToNewPos();
        }
    }
    */
}