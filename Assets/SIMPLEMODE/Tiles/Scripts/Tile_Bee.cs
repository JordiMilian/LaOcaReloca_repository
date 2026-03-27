using UnityEngine;
using System.Collections;
using static StringTools;
using System.Collections.Generic;
public class Tile_Bee : Tile_Insect
{
    List<Tile_Plant> modifiedPlants = new();
    public override IEnumerator OnPlacedInBoard()
    {
        yield return base.OnPlacedInBoard();
        BoardController.OnBoardModified.AddListener(OnBoardModified);
    }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    //public override string GetTooltipText() { }

    void OnBoardModified()
    {
        foreach(Tile_Plant plant in modifiedPlants)
        {
            plant.growthModifiers -= plantModify;
        }
        modifiedPlants.Clear();

        List<TileController> adjacentTiles = MathJ.GetAdjacentTiles(_Controller,1,true);
        foreach (TileController tile in adjacentTiles)
        {
            if(tile._Profile is Tile_Plant)
            {
                Tile_Plant plant = (Tile_Plant)tile._Profile;
                plant.growthModifiers += plantModify;
                modifiedPlants.Add(plant);
            }
        }

    }

    float plantModify(float growth)
    {
        return growth * 3;
    }
    public override IEnumerator OnLandedFly()
    {
        yield return base.OnLandedFly();
        List<TileController> adjacentTiles = MathJ.GetAdjacentTiles(_Controller);
        foreach (TileController tile in adjacentTiles)
        {
            yield return tile.AddBaseDamage(1);
        }

    }
}