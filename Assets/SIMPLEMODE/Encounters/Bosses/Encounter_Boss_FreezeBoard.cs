using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Encounter_Boss_FreezeBoard : Encounter_Boss
{
    List<TileController> frozenTiles = new();
    List<TileController> shuffledTiles = new();
    public override void ActivateSpecialBossEffect()
    {
        Board_Controller_simple board = Board_Controller_simple.Instance;

        for (int i = board.TilesList.Count - 2; i >= 1; i--)
        {
            TileController tile = board.TilesList[i];
            board.TilesList.RemoveAt(i);
            shuffledTiles.Add(tile);
        }
        for (int i = shuffledTiles.Count - 1; i >= 0; i--)
        {
            TileController tile = shuffledTiles[i];
            int randomIndex = Random.Range(1, board.TilesList.Count - 1);
            board.TilesList.Insert(randomIndex, tile);
        }
        board.UpdateStructData();
        board.MoveTiles_ToTfData(true);

        foreach (TileController tile in board.TilesList)
        {
            if (tile._Info is Tile_Start || tile._Info is Tile_End) { continue; }

            if (!tile._Info.genericSkills.Contains(GenericSkills.Unmovable))
            {
                tile._Info.AddGenericSkill(GenericSkills.Unmovable);
                frozenTiles.Add(tile);
            }
            else
            {
                Debug.Log("not unmovable found, named: "+ tile._Info.GetType());
            }
        }
    }
    public override void DeactivateSpecialBossEffect()
    {
        foreach (TileController tile in frozenTiles)
        {
            tile._Info.RemoveGenericSkill(GenericSkills.Unmovable);
        }
        frozenTiles.Clear();
    }
    public override float GetBossHealth(float baseHP)
    {
        return baseHP * 2;
    }

    public override string GetTooltipDescription()
    {
        return "Randomize and freeze board before starting";
    }
}
