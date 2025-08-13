using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
public class Tile_TilesShuffler : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    [SerializeField] float MultiplierPerShuffledTile = 2;
  public override IEnumerator OnPlayerStepped()
    {
        if (indexInBoard < BoardController.TilesList.Count - 2) //si no es la penultima
        {
            List<Tile_Base> tilesToShuffle = new();
            for (int i = BoardController.TilesList.Count - 2; i > BoardController.PlayerIndex + 1; i--)
            {
                Tile_Base tile = BoardController.TilesList[i];
                tilesToShuffle.Add(tile);
                BoardController.TilesList.RemoveAt(i);
            }

            for (int i = tilesToShuffle.Count - 1; i >= 0; i--)
            {
                Tile_Base tile = tilesToShuffle[i];
                int randomIndex = Random.Range(BoardController.PlayerIndex + 1, BoardController.TilesList.Count - 1);
                BoardController.TilesList.Insert(randomIndex, tile);
            }
            BoardController.UpdateTfData_ByTilesList();

            BoardController.MoveTiles_ToTfData(true);

            yield return new WaitForSeconds(0.5f);
            yield return GameController.Co_AddAcumulatedMultiplier(MultiplierPerShuffledTile * tilesToShuffle.Count);
        }
        yield return base.OnPlayerStepped(); 

    }
    public override string GetTooltipText() { return $"{On.OnCrossed} Shuffle tiles forward. Add {MathJ.AddMultiplier(MultiplierPerShuffledTile)} multiplier per Shuffled tile"; }
}