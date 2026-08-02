using UnityEngine;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
public class Tile_RatKing : TileInfo
{
    [SerializeField] float ratsDamageAdder = .5f;
    [SerializeField] float addedAmountOnLanded = .5f;

    public override TileInfo GetCopy()
    {
        Tile_RatKing newInfo = (Tile_RatKing)CopyBaseStatsIntoOther(new Tile_RatKing());
        newInfo.ratsDamageAdder = ratsDamageAdder;
        newInfo.addedAmountOnLanded = addedAmountOnLanded;
        return newInfo;
    }
    public override IEnumerator OnPlacedInBoard()
    {
        yield return base.OnPlacedInBoard();

        List<TileController> currentRats = MathJ.GetAllTilesWithTag(TileTags.Rat, _Controller, true);
        foreach (TileController rat in currentRats)
        {
            modifyRat(rat);
        }

        BoardController.OnAddedTile.AddListener(OnAddedTile);
        BoardController.OnRemovedTile.AddListener(OnRemovedTile);
    }
    public override IEnumerator OnRemovedFromBoard() 
    {
        yield return base.OnRemovedFromBoard();

        for (int i = modifiedRats.Count -1; i <= 0; i--)
        {
            unmodifyRat(modifiedRats[i]);
        }
        BoardController.OnAddedTile.RemoveListener(OnAddedTile);
        BoardController.OnRemovedTile.RemoveListener(OnRemovedTile);
    }
    public override IEnumerator OnPlayerLanded() 
    { 
        yield return base.OnPlayerLanded();
        ratsDamageAdder += addedAmountOnLanded;
        
    }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override string GetTooltipText() { return $"Other Rats deal {ratsDamageAdder * 100}% more DMG \n {OnLanded} Increase that amount by {addedAmountOnLanded * 100}%"; }

    List<TileController> modifiedRats = new();
    void OnAddedTile(TileController newTile)
    {
        if(newTile == _Controller) { return; }
        if(newTile._Info.tileTags.Contains(TileTags.Rat))
        {
            modifyRat(newTile);
        }
    }
    void OnRemovedTile(TileController removedTile)
    {
        if (removedTile == _Controller) { return; }
        if (removedTile._Info.tileTags.Contains(TileTags.Rat))
        {
            unmodifyRat(removedTile);
        }
    }
    void modifyRat(TileController rat) { rat.BaseDamageModifiers += ratModifier; modifiedRats.Add(rat); }
    void unmodifyRat(TileController rat) { rat.BaseDamageModifiers -= ratModifier;modifiedRats.Remove(rat); }
    float ratModifier(float dmg)
    {
        return dmg * (1 + ratsDamageAdder);
    }
}