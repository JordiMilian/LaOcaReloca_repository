using UnityEngine;
using System.Collections;
public class Tile_RatQueen : TileInfo
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    
    [SerializeField] TileConfig ratTokenTileProfile;
    [SerializeField] int ratsAmountOnLanded = 3;
    public override IEnumerator OnPlayerStepped()
    { 
        yield return base.OnPlayerStepped();
        yield return CreateRandomRat();
    }
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        for (int i = 0; i < ratsAmountOnLanded; i++)
        {
            yield return CreateRandomRat();
        }
    }
    IEnumerator CreateRandomRat()
    {
        TileController ratTokenController = TilesFactory.instance.InstantiateTile(ratTokenTileProfile._configInfo);
        ratTokenController.transform.position = _Controller.transform.position;
        int randomIndex, ownIndex = _Controller.indexInBoard;
        do
        {
            randomIndex = MathJ.GetRandomIndexInBoard(true);
        }
        while (randomIndex == ownIndex);
       
        yield return BoardController.C_AddNewTile(ratTokenController, randomIndex);
    }
    public override TileInfo GetCopy()
    {
        Tile_RatQueen newInfo = (Tile_RatQueen)CopyBaseStatsIntoOther(new Tile_RatQueen());
        newInfo.ratTokenTileProfile = ratTokenTileProfile;
        newInfo.ratsAmountOnLanded = ratsAmountOnLanded;
        return newInfo;
    }
    public override string GetTooltipText() { return $"{OnCrossed} Create a RAT TOKEN \n {OnLanded} Create {ratsAmountOnLanded} RAT TOKENS"; }
}