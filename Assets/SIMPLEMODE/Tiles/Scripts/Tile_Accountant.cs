using System.Collections;
using UnityEngine;

public class Tile_Accountant : Tile_Profile
{
    [SerializeField] int damagePerIndex;
    public override IEnumerator OnPlayerStepped()
    {
        Tile.DamagesToDeal.Add(BoardController.TilesList.Count * damagePerIndex);
        yield return base.OnPlayerStepped();
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Deal {MathJ.AddDamage(damagePerIndex)} per tile in board ({BoardController.TilesList.Count * damagePerIndex})";
    }
}
