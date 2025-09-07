using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "TileProfile/Shuffler", fileName = "TileProfile_Shuffler")]
public class Tile_Shuffler : Tile_Profile
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    [SerializeField] float DmgPerShuffledTile = 2;
  public override IEnumerator OnPlayerStepped()
    {
        if (Tile.indexInBoard < BoardController.TilesList.Count - 2) //si no es la penultima
        {
            List<TileController> tilesToShuffle = new();
            for (int i = BoardController.TilesList.Count - 2; i > BoardController.PlayerIndex + 1; i--)
            {
                TileController tile = BoardController.TilesList[i];
                tilesToShuffle.Add(tile);
                BoardController.TilesList.RemoveAt(i);
            }

            for (int i = tilesToShuffle.Count - 1; i >= 0; i--)
            {
                TileController tile = tilesToShuffle[i];
                int randomIndex = Random.Range(BoardController.PlayerIndex + 1, BoardController.TilesList.Count - 1);
                BoardController.TilesList.Insert(randomIndex, tile);
            }
            BoardController.UpdateStructData();

            BoardController.MoveTiles_ToTfData(true);

            yield return new WaitForSeconds(0.5f);
            Tile.DamagesToDeal.Add(DmgPerShuffledTile * tilesToShuffle.Count);
        }
        yield return base.OnPlayerStepped(); 

    }
    public override string GetTooltipText() { return $"{OnCrossed} Shuffle TILES forward. Deal {MathJ.AddDamage(DmgPerShuffledTile)} per Shuffled tile"; }
}