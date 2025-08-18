using System.Collections;
using UnityEngine;

public class Tile_Oca_ForeachOcaGetDamage : Tile_Oca
{
    [SerializeField] int damagePerOca = 1;
    public override IEnumerator OnPlayerLanded()
    {
        int OcasCount = 0;
        foreach (Tile_Base tile in BoardController.TilesList)
        {
            if (tile is Tile_Oca) { OcasCount++; }
        }
        yield return GameController.C_AddAcumulatedDamage(damagePerOca * OcasCount);
        Debug.Log($"Found {OcasCount} ocas");

        yield return base.OnPlayerLanded();
    }
    public override string GetTooltipText()
    {
        return $"{MathJ.BoldText("ON LANDED: ")} Add {damagePerOca} damage per Oca and gain {GameController.MoneyToRoll} money";
    }
}
