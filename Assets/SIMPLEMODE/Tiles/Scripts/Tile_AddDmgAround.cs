using UnityEngine;
using System.Collections;
public class Tile_AddDmgAround : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

    public override string GetTooltipText() 
    {
        return$"ON CROSSED: Add {addedDmg} damage to tiles around";

    }

    [SerializeField] float addedDmg;
    public override IEnumerator OnPlayerStepped()
    { 
        yield return base.OnPlayerStepped(); 
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the tile itself

                Vector2Int aroundTileIndex = vectorInBoard + new Vector2Int(i, j);
                if (BoardController.TilesByPosition.ContainsKey(aroundTileIndex))
                {
                    Tile_Base adjacentTile = BoardController.TilesByPosition[vectorInBoard + new Vector2Int(i, j)];
                    if(adjacentTile is Tile_Start) { continue; } 
                    adjacentTile.AddDefaultCrossingDamage(addedDmg);
                }

            }
        }
    }
}