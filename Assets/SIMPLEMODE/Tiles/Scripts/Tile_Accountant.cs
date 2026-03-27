using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/Accountant", fileName = "Tile_Accountant")]
public class Tile_Accountant : TileInfo
{
    [SerializeField] int damagePerIndex = 1;
    public override IEnumerator OnPlayerLanded()
    {
        damagePerIndex++;
        yield return base.OnPlayerLanded();
    }
    public override IEnumerator OnTileFinished()
    {
        _Controller.DamagesToDeal.Add(BoardController.TilesList.Count * damagePerIndex);
        return base.OnTileFinished();
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
