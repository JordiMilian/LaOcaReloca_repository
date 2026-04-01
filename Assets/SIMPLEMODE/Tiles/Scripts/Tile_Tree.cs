using UnityEngine;
using System.Collections;
using static StringTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
public class Tile_Tree : Tile_Plant
{
    List<Tile_Plant> subscribedPlants = new();
    [SerializeField] float growthMultiplier = 1.5f;
    public override IEnumerator OnPlacedInBoard() 
    { 
        yield return base.OnPlacedInBoard(); 
        BoardController.OnBoardModified.AddListener(OnModifiedBoard);
        subscribeToAdjacentPlants();
    }
    public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();
        BoardController.OnBoardModified.RemoveListener(OnModifiedBoard);
        unsubscribeFromCurrent();
    }
    void OnModifiedBoard()
    {
        unsubscribeFromCurrent();

        subscribeToAdjacentPlants();
    }
    void unsubscribeFromCurrent()
    {
        foreach (Tile_Plant plant in subscribedPlants)
        {
            plant.growthModifiers -= MultiplyGrowth;
        }
        subscribedPlants.Clear();
    }
    void subscribeToAdjacentPlants()
    {
        List<TileController> adjacentTiles = MathJ.GetAdjacentTiles(_Controller);
        foreach (TileController tile in adjacentTiles)
        {
            if (tile._Info.tileTags.Contains(TileTags.Plant))
            {
                Tile_Plant plantProfile = (Tile_Plant)tile._Info;
                subscribedPlants.Add(plantProfile);
                plantProfile.growthModifiers += MultiplyGrowth;
            }
        }
    }
    float MultiplyGrowth(float growth)
    {
        return growth * growthMultiplier;
    }
   //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return base.GetTooltipText() + $"\nAdjancent Plants grow {(growthMultiplier - 1) * 100}% more"; }
}