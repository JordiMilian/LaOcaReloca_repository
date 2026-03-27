using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Perfume : TileInfo
{
    [SerializeField] float perfumeDamage = 5;
    [SerializeField] float increaseDmg = 2;
    bool isPerfumed;
   public override IEnumerator OnPlacedInBoard() 
    { 
        yield return base.OnPlacedInBoard();
        GameController.OnRolledDice.AddListener(UnsubscribeToCrossed);
    }
   public override IEnumerator OnRemovedFromBoard() 
    { 
        yield return base.OnRemovedFromBoard();
        GameController.OnRolledDice.RemoveListener(UnsubscribeToCrossed);
    }
   public override IEnumerator OnPlayerLanded() 
    { 
        yield return base.OnPlayerLanded();
        perfumeDamage += increaseDmg;
    }
   public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped();

        isPerfumed = true;
        GameController.OnCrossed_CardEffects.AddEffect(OnCrossedTileWhilePerfumed);
    }
    IEnumerator OnCrossedTileWhilePerfumed(TileController tile)
    {
        if(isPerfumed)
        {
            yield return tile.AddBaseDamage(perfumeDamage);
        }
    }
    void UnsubscribeToCrossed()
    {
        if (isPerfumed)
        {
            GameController.OnCrossed_CardEffects.RemoveEffect(OnCrossedTileWhilePerfumed);
            isPerfumed = false;
        }
    }

   public override string GetTooltipText() {
        return $"{OnCrossed} Every Tile crossed afterwards gets {StringTools.AddDamageString(perfumeDamage)}" +
            $"\n{OnLanded} Increase that amount by {increaseDmg}";
    }
}