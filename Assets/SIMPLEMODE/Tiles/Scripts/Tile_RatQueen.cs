using UnityEngine;
using System.Collections;
public class Tile_RatQueen : Tile_Profile
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    
    [SerializeField] Tile_Profile ratTokenTileProfile;
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
        TileController ratTokenController = TilesFactory.instance.InstantiateTile(ratTokenTileProfile);
        ratTokenController.transform.position = _Tile.transform.position;
        int randomIndex, ownIndex = _Tile.indexInBoard;
        do
        {
            randomIndex = MathJ.GetRandomIndexInBoard(true);
        }
        while (randomIndex == ownIndex);
       
        yield return BoardController.C_AddNewTile(ratTokenController, randomIndex);
    }
   public override string GetTooltipText() { return $"{OnCrossed} Create a RAT TOKEN \n {OnLanded} Create {ratsAmountOnLanded} RAT TOKENS"; }
}