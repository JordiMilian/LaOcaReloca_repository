using System.Collections;
using UnityEngine;

public class Tile_Oca_ForeachOcaGetDamage : Tile_Oca
{
    [SerializeField] int damagePerOca = 1;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        int OcasCount = 0;
        foreach (Tile_Base tile in BoardController.TilesList)
        {
            if (tile is Tile_Oca) { OcasCount++; }
        }
        GameController.AddAcumulatedDamage(damagePerOca * OcasCount);
        Debug.Log($"Found {OcasCount} ocas");
    }
    public override string GetTooltipText()
    {
        return $"(Oca) On Landed: Add {damagePerOca} damage per Oca";
    }
}
