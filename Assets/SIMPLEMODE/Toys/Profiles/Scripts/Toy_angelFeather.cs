using UnityEngine;
using System.Collections;
using static StringTools;
public class Toy_angelFeather : Toy_Profile
{
   public override void OnActivatedToy()
{
        _gameController.MaxRollsPerEncounter += 1;
        _gameController.RollsRemaining += 1;
        _gameController.UpdateRemainingRolls();
    }
   public override void OnDeactivatedToy()
    {
        _gameController.MaxRollsPerEncounter -= 1;
        _gameController.RollsRemaining -= 1;
        _gameController.UpdateRemainingRolls();
    }
  public override string GetTooltipDescription() { return "+1 extra dice roll per encounter"; }
}