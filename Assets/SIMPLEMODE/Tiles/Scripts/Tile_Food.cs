using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Food : TileStateClass
{
    public int currentRot, baseRot;

    IEnumerator OnRolled()
    {
        currentRot--;
        if(currentRot <= 0)
        {
            yield return OnRotten();
        }
    }
    public virtual IEnumerator OnRotten()
    {
        yield return BoardController.C_RemoveTile(_Tile.indexInBoard);
    }
    public virtual IEnumerator OnEaten()
    {
        yield return _Tile.C_DealAllDamageToDeal();
        yield return BoardController.C_RemoveTile(_Tile.indexInBoard);
    }
   public override IEnumerator OnPlacedInBoard()
    {
        currentRot = baseRot;
        yield return base.OnPlacedInBoard();
        GameController.OnRolledDice_CardEffects.AddEffect(OnRolled);
    }
   public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnRolledDice_CardEffects.RemoveEffect(OnRolled);
    } 
   public override IEnumerator OnPlayerLanded() 
    {
        yield return base.OnPlayerLanded();
        yield return OnEaten();
    }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return Rot(currentRot); }
}