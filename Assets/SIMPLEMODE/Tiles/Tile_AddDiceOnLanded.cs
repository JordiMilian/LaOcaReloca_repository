using System.Collections;
using UnityEngine;

public class Tile_AddDiceOnLanded : Tile_Base
{
    [SerializeField] GameObject DicePrefab;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        GameObject newDice = Instantiate(DicePrefab, Dices_Controller.Instance.transform);
        Dices_Controller.Instance.availableDices.Add(newDice.GetComponent<Dice>());
    }
}
