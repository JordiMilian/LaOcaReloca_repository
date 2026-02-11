using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static StringTools;
using NUnit.Framework;
using Unity.VisualScripting;
public class Tile_RacoonGang : Tile_Profile
{
    [SerializeField] int AmountToTake = 1;
    [SerializeField] float dmgPerMoney = 15;
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
            CoroutineRunner.instance.StartCoroutine( tile.AddBaseDamage(tmpAmount * dmgPerMoney));
        }
        yield return new WaitForSeconds(MessajesManager.instance.GetDurationMultiplier());
    }
   public override string GetTooltipText() { return $"{OnCrossed} Loose {AmountToTake} MONEY and increase {AddDamageString(dmgPerMoney)} Adjacent Tiles\n{OnLanded} Spawn {racoonsToCreate} Racoons"; }

    IEnumerator CreateRandomRacoon()
    {
        TileController ratTokenController = TilesFactory.instance.InstantiateTile(racoonToken_Profile);
        ratTokenController.transform.position = _Tile.transform.position;

        int randomIndex = MathJ.GetRandomIndexInBoard(true);
        yield return BoardController.C_AddNewTile(ratTokenController, randomIndex);
    }
}