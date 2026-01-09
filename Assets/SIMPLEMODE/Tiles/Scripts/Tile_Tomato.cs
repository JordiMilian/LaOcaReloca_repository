using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Tomato : Tile_Food
{
    [SerializeField] Tile_Profile TreeProfile;
    [SerializeField] int moneyOnEaten = 10;
    [SerializeField] float chanceToSpawnPlant = .5f;
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override string GetTooltipText() 
    {
        return base.GetTooltipText() +
            $"\n{StringTools.OnEaten} Get +{moneyOnEaten} money" +
            $"\n{StringTools.OnRotten} {chanceToSpawnPlant * 100}% chance to spawn a Tomato Plant";
    }

    public override IEnumerator OnRotten()
    {
        int indexInBoard = _Tile.indexInBoard;
        yield return base.OnRotten();

        if(Random.Range(0f,1f) < chanceToSpawnPlant)
        {
            TileController treeTile = TilesFactory.instance.InstantiateTile(TreeProfile);
            yield return BoardController.C_AddNewTile(treeTile, indexInBoard);

        }


    }
    public override IEnumerator OnEaten()
    {
        GameController.AddMoney(moneyOnEaten);
        return base.OnEaten();
    }
}