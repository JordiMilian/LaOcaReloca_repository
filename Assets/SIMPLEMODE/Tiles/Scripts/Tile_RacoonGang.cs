using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static StringTools;
using NUnit.Framework;
public class Tile_RacoonGang : Tile_Profile
{
    [SerializeField] int AmountToTake = 5;
    [SerializeField] float dmgPerMoney = 3;
    [SerializeField] int racoonsToCreate = 2;
    [SerializeField] Tile_Profile racoonToken_Profile;
    //public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); }
    //public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); }
    public override IEnumerator OnPlayerLanded()
   { 
        yield return base.OnPlayerLanded();
        for (int i = 0; i < racoonsToCreate; i++)
        {
            yield return CreateRandomRacoon();
        }

    }
   public override IEnumerator OnPlayerStepped()
    { 
        yield return base.OnPlayerStepped();
        int tmpAmount = AmountToTake;
        if(GameController.GetCurrentMoney() < AmountToTake)
        {
            tmpAmount = GameController.GetCurrentMoney();
        }
        GameController.RemoveMoney(tmpAmount);

        List<TileController> tilesAround = MathJ.GetAdjacentTiles(_Tile, 1);
        foreach (TileController tile in tilesAround)
        {
            tile.AddBaseDamage(tmpAmount * dmgPerMoney);
        }
    }
   public override string GetTooltipText() { return $"{OnCrossed} Loose up to {AmountToTake} MONEY and add {AddDamage(dmgPerMoney)} per MONEY lost to Adjacent Tiles\n{OnLanded} Spawn {racoonsToCreate} Racoons"; }

    IEnumerator CreateRandomRacoon()
    {
        TileController ratTokenController = TilesFactory.instance.InstantiateTile(racoonToken_Profile);
        ratTokenController.transform.position = _Tile.transform.position;

        int randomIndex = Random.Range(1, BoardController.TilesList.Count - 1);
        yield return BoardController.C_AddNewTile(ratTokenController, randomIndex);
    }
}