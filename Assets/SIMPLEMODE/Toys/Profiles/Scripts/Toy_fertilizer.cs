using UnityEngine;
using System.Collections;
using static StringTools;
using System.Collections.Generic;
using System;
public class Toy_fertilizer : Toy_Profile
{
    List<Tile_Plant> subscribedPlants = new();
    [SerializeField] float growthMultiplier = 1.5f;
    public override void OnActivatedToy()
    {
        subscribedPlants = new();
        List<TileController> tilePlants = MathJ.GetAllTilesWithTag(TileTags.Plant,null, false);
        foreach(TileController tile in tilePlants) { subscribedPlants.Add((Tile_Plant)tile._Info); }

        foreach (Tile_Plant plant in subscribedPlants)
        {
            modifyGrowth(plant);
        }

        _boardController.OnAddedTile.AddListener(OnAddedTile);
        _boardController.OnRemovedTile.AddListener(OnRemovedTile);
    }
    public override void OnDeactivatedToy()
    {
        foreach (Tile_Plant plant in subscribedPlants)
        {
            removeModify(plant);
        }
        subscribedPlants.Clear();

        _boardController.OnAddedTile.RemoveListener(OnAddedTile);
        _boardController.OnRemovedTile.RemoveListener(OnRemovedTile);
    }

    void modifyGrowth(Tile_Plant plant) { plant.growthModifiers += MultiplyGrowth; }
    void removeModify(Tile_Plant plant) { plant.growthModifiers -= MultiplyGrowth; }
    float MultiplyGrowth(float growth)
    {
        return growth * growthMultiplier;
    }

    private void OnAddedTile(TileController newTile)
    {
        if(newTile._Info is Tile_Plant)
        {
            
            modifyGrowth((Tile_Plant)newTile._Info);
        }
    }
    void OnRemovedTile(TileController removedTile)
    {
        if (removedTile._Info is Tile_Plant)
        {
            removeModify((Tile_Plant)removedTile._Info);
        }
    }
   
  public override string GetTooltipDescription() { return $"All plants grow {(growthMultiplier - 1)*100}% more"; }
}