using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "TileProfile/Shuffler", fileName = "TileProfile_Shuffler")]
public class Tile_Shuffler : TileInfo
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }
    [SerializeField] float DmgPerShuffledTile = 2;
    [SerializeField] float ExtraDMGOnLanded = 2;

    public override TileInfo GetCopy()
    {
        Tile_Shuffler newInfo = (Tile_Shuffler)CopyBaseStatsIntoOther(new Tile_Shuffler());
        newInfo.DmgPerShuffledTile = DmgPerShuffledTile;
        newInfo.ExtraDMGOnLanded = ExtraDMGOnLanded;
        return newInfo;
    }
    public override IEnumerator OnPlayerLanded()
    {
        DmgPerShuffledTile += ExtraDMGOnLanded;
        return base.OnPlayerLanded();
    }
    public override IEnumerator OnTileFinished()
    {
        if (_Controller.indexInBoard < BoardController.TilesList.Count - 2) //si no es la penultima
        {
            List<TileController> tilesToShuffle = new();
            for (int i = BoardController.TilesList.Count - 2; i > _Controller.indexInBoard + 1; i--)
            {
                TileController tile = BoardController.TilesList[i];
                tilesToShuffle.Add(tile);
                BoardController.TilesList.RemoveAt(i);
            }

            for (int i = tilesToShuffle.Count - 1; i >= 0; i--)
            {
                TileController tile = tilesToShuffle[i];
                int randomIndex = Random.Range(_Controller.indexInBoard + 1, BoardController.TilesList.Count - 1);
                BoardController.TilesList.Insert(randomIndex, tile);
            }
            BoardController.UpdateStructData();

            BoardController.MoveTiles_ToTfData(true);

            yield return new WaitForSeconds(0.5f);
            _Controller.DamagesToDeal.Add(DmgPerShuffledTile * tilesToShuffle.Count);
        }
        yield return base.OnTileFinished();
    }
    float GetShuffledDmg()
    {
        if(BoardController == null || _Controller.tileState != TileState.InBoard) {  return 0f; }
        int tilesForwardCount = (BoardController.TilesList.Count - 2) - (_Controller.indexInBoard + 1);
        return tilesForwardCount * DmgPerShuffledTile;

    }
    public override string GetTooltipText() {
        return $"{OnCrossed} Shuffle TILES forward. Deal {MathJ.AddDamage(DmgPerShuffledTile)} per Shuffled tile ({GetShuffledDmg()})" +
            $"\n{OnLanded} Increase that amount by {ExtraDMGOnLanded};"; }
}