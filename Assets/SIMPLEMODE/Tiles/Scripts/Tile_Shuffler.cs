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
    [SerializeField] float ExtraDMGOnLanded = 2;
    public override IEnumerator OnPlayerLanded()
    {
        DmgPerShuffledTile += ExtraDMGOnLanded;
        return base.OnPlayerLanded();
    }
    public override IEnumerator OnTileFinished()
    {
        if (_Tile.indexInBoard < BoardController.TilesList.Count - 2) //si no es la penultima
        {
            List<TileController> tilesToShuffle = new();
            for (int i = BoardController.TilesList.Count - 2; i > _Tile.indexInBoard + 1; i--)
            {
                TileController tile = BoardController.TilesList[i];
                tilesToShuffle.Add(tile);
                BoardController.TilesList.RemoveAt(i);
            }

            for (int i = tilesToShuffle.Count - 1; i >= 0; i--)
            {
                TileController tile = tilesToShuffle[i];
                int randomIndex = Random.Range(_Tile.indexInBoard + 1, BoardController.TilesList.Count - 1);
                BoardController.TilesList.Insert(randomIndex, tile);
            }
            BoardController.UpdateStructData();

            BoardController.MoveTiles_ToTfData(true);

            yield return new WaitForSeconds(0.5f);
            _Tile.DamagesToDeal.Add(DmgPerShuffledTile * tilesToShuffle.Count);
        }
        yield return base.OnTileFinished();
    }
    float GetShuffledDmg()
    {
        if(BoardController == null || _Tile.tileState != TileState.InBoard) {  return 0f; }
        int tilesForwardCount = (BoardController.TilesList.Count - 2) - (_Tile.indexInBoard + 1);
        return tilesForwardCount * DmgPerShuffledTile;

    }
    public override string GetTooltipText() {
        return $"{OnCrossed} Shuffle TILES forward. Deal {MathJ.AddDamage(DmgPerShuffledTile)} per Shuffled tile ({GetShuffledDmg()})" +
            $"\n{OnLanded} Increase that amount by {ExtraDMGOnLanded};"; }
}