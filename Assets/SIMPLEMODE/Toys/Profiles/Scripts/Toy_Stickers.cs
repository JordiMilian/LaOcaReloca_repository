using UnityEngine;
using System.Collections;
using static StringTools;
public class Toy_Stickers : Toy_Info
{
    [SerializeField] int valueAdded = 2;
   public override void OnActivatedToy()
    {
        _gameController.dicesController.AddBoughtValue(valueAdded);
        _gameController.OnRolledDice.AddListener(OnRolledDice);
    }
   public override void OnDeactivatedToy()
    {
        _gameController.dicesController.AddBoughtValue(-valueAdded);
        _gameController.OnRolledDice.RemoveListener(OnRolledDice);
    }

    void OnRolledDice()
    {
        _gameController.dicesController.AddBoughtValue(valueAdded);
    }
  public override string GetTooltipDescription() { return $"Roll +{valueAdded} on every dice roll"; }
}