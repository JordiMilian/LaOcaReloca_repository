using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Food : TileInfo
{
    [HideInInspector] public int currentRot;
    public int baseRot;

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
        yield return BoardController.C_RemoveTile(_Controller.indexInBoard);
    }
    public virtual IEnumerator OnEaten()
    {
        yield return _Controller.C_DealAllDamageToDeal();
        yield return BoardController.C_RemoveTile(_Controller.indexInBoard);
    }
   public override void EnableExtraLogic()
    {
        currentRot = baseRot;
        GameController.OnRolledDice_CardEffects.AddEffect(OnRolled);
    }
   public override void DisableExtraLogic()
    {
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