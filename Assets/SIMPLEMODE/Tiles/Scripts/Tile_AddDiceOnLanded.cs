using System.Collections;
using UnityEngine;

public class Tile_AddDiceOnLanded : Tile_Profile
{
    [SerializeField] GameObject DicePrefab_OnCrossed;
    [SerializeField] GameObject DicePrefab_OnLanded;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();

        Dices_Controller.Instance.SpawnNewDice(DicePrefab_OnLanded);
    }
    public override IEnumerator OnPlayerStepped()
    {
        yield return base.OnPlayerStepped();
        Dices_Controller.Instance.SpawnNewDice(DicePrefab_OnCrossed);
    }
    public override string GetTooltipText()
    {
        return $"{OnLanded} Add a {DicePrefab_OnCrossed.name} \n{OnCrossed} Add a {DicePrefab_OnLanded.name}";
    }
}
