using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/Accountant", fileName = "Tile_Accountant")]
public class Tile_Accountant : Tile_Profile
{
    [SerializeField] int damagePerIndex = 1;
    public override IEnumerator OnPlayerStepped()
    {
        _Tile.DamagesToDeal.Add(BoardController.TilesList.Count * damagePerIndex);
        yield return base.OnPlayerStepped();
    }
    public override IEnumerator OnPlayerLanded()
    {
        damagePerIndex++;
        _Tile.DamagesToDeal.Add(BoardController.TilesList.Count);
        yield return base.OnPlayerLanded();
    }

    public override string GetTooltipText()
    {
        float amountForDisplay = 0;
        if(BoardController != null)
        {
            amountForDisplay = BoardController.TilesList.Count * damagePerIndex;
        }
        return $"{OnCrossed} Deal {MathJ.AddDamage(damagePerIndex)} per tile in board ({amountForDisplay}) \n " +
            $"{OnLanded} Increase +1 that amount";
    }
}
