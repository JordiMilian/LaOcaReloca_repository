using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/DicesEnjoyers/DicesCreator", fileName = "Tile_DicesCreator")]
public class Tile_AddDiceOnLanded : TileStateClass
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
        return $"{OnLanded} Add a D6 \n{OnCrossed} Add a Single-Use D6";
    }
}
