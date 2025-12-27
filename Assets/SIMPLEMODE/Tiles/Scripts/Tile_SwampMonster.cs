using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
public class Tile_SwampMonster : Tile_Profile
{
    [SerializeField] Tile_Profile swampTokenProfile;
    [SerializeField] float DamageAddedOnCross = 100;
    public override IEnumerator OnPlacedInBoard() 
   {
        yield return base.OnPlacedInBoard();
        RemoveTileTag(this, TileTags.Unmovable);

        TileController newSwamp01 = TilesFactory.instance.InstantiateTile(swampTokenProfile);
        newSwamp01.transform.position = _Tile.transform.position;

        yield return BoardController.C_AddNewTile(newSwamp01, _Tile.indexInBoard);
        RemoveTileTag(newSwamp01._Profile, TileTags.Unmovable);

        TileController newSwamp02 = TilesFactory.instance.InstantiateTile(swampTokenProfile);
        newSwamp02.transform.position = _Tile.transform.position;

        yield return BoardController.C_AddNewTile(newSwamp02, _Tile.indexInBoard +1);
        RemoveTileTag(newSwamp02._Profile, TileTags.Unmovable);

        AddTileTag(this, TileTags.Unmovable);
        AddTileTag(newSwamp01._Profile, TileTags.Unmovable);
        AddTileTag(newSwamp02._Profile, TileTags.Unmovable);
    }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

    public static void RemoveTileTag(Tile_Profile profile, TileTags tag)
    {
        List<TileTags> tagsList = profile.tileTags.ToList();
        tagsList.Remove(tag);
        profile.tileTags = tagsList.ToArray();

    }
    public static void AddTileTag(Tile_Profile profile, TileTags tag)
    {
        List<TileTags> tagsList = profile.tileTags.ToList();
        tagsList.Add(tag);
        profile.tileTags = tagsList.ToArray();
    }
   public override IEnumerator OnPlayerStepped() 
   { 
        _Tile.AddBaseDamage(DamageAddedOnCross);
        yield return base.OnPlayerStepped();
    }
   public override string GetTooltipText() { return $"{StringTools.Unmovable}\n{OnEnterInBoard} Spawn 2 SWAMPS around \n {OnCrossed} Add {MathJ.AddDamage(DamageAddedOnCross)} to this tile"; }
}