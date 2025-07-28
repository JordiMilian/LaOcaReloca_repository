using System.Collections;
using UnityEngine;

public class Tile_AddDiceOnLanded : Tile_Base
{
    [SerializeField] GameObject DicePrefab;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();

        Dices_Controller.Instance.SpawnNewDice(DicePrefab);
    }
    public override string GetTooltipText()
    {
        return $"On Landed: Add a {DicePrefab.name} dice";
    }
}
