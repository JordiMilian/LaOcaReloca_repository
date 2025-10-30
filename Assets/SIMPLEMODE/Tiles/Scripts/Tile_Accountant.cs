using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/Accountant", fileName = "Tile_Accountant")]
public class Tile_Accountant : Tile_Profile
{
    [SerializeField] int damagePerIndex = 1;
    public override IEnumerator OnPlayerStepped()
    {
        Tile.DamagesToDeal.Add(BoardController.TilesList.Count * damagePerIndex);
        yield return base.OnPlayerStepped();
    }
    public override IEnumerator OnPlayerLanded()
    {
        damagePerIndex++;
        Tile.DamagesToDeal.Add(BoardController.TilesList.Count);
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
            $"{OnLanded} Deal +1 MORE per tile in board permanently";
    }
}
