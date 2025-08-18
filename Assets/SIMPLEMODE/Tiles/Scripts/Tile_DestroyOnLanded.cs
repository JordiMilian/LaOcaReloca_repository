using UnityEngine;
using System.Collections;
public class Tile_DestroyOnLanded : Tile_Base
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }

    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    [SerializeField] float MultiplierAdded = 20;
    public override string GetTooltipText() 
    { return $"{On.OnLanded} Destroy a random tile and {MathJ.AddMultiplier(MultiplierAdded)}"; }

    public override IEnumerator OnPlayerLanded()
    {
        yield return GameController.C_AddAcumulatedMultiplier(MultiplierAdded);
        yield return base.OnPlayerLanded();

        Tile_Base randomTile = null;
        int randomIndex = 0;
        while(randomTile == null || randomTile is Tile_Start || randomTile is Tile_End || randomTile == this)
        {
            randomIndex = Random.Range(0, BoardController.TilesList.Count);
            randomTile = BoardController.TilesList[randomIndex];
        }
        BoardController.RemoveTile(randomIndex);

        
    }
}