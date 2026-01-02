using UnityEngine;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
public class Tile_RatKing : Tile_Profile
{
    [SerializeField] float ratsDamageAdder = .5f;
    [SerializeField] float addedAmountOnLanded = .5f;
    public override IEnumerator OnPlacedInBoard()
    {
        yield return base.OnPlacedInBoard();

        List<TileController> currentRats = MathJ.GetAllTilesWithTag(TileTags.Rat, _Tile, true);
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
        if(newTile == _Tile) { return; }
        if(newTile._Profile.tileTags.Contains(TileTags.Rat))
        {
            modifyRat(newTile);
        }
    }
    void OnRemovedTile(TileController removedTile)
    {
        if (removedTile == _Tile) { return; }
        if (removedTile._Profile.tileTags.Contains(TileTags.Rat))
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