using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Tomato : Tile_Food
{
    [SerializeField] int moneyOnEaten = 10, moneyOnRotten = 1;
    [SerializeField] float chanceToSpawnPlant = .5f;
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override string GetTooltipText() 
    {
        return base.GetTooltipText() +
            $"\n{StringTools.OnEaten} Get +{moneyOnEaten} money" +
            $"\n{StringTools.OnRotten} Get +{moneyOnRotten} money";
    }

    public override IEnumerator OnRotten()
    {
        yield return base.OnRotten();
        GameController.AddMoney(moneyOnEaten);
    }
    public override IEnumerator OnEaten()
    {
        GameController.AddMoney(moneyOnEaten);
        return base.OnEaten();
    }
}