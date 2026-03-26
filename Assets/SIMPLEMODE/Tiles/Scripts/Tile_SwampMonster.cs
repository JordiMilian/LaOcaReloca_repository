using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
public class Tile_SwampMonster : TileStateClass
{
    [SerializeField] TileStateClass swampTokenProfile;
    [SerializeField] float DamageAddedOnCross = 100;
    public override IEnumerator OnPlacedInBoard() 
   {
        yield return base.OnPlacedInBoard();
        RemoveSkill(this, GenericSkills.Unmovable);

        TileController newSwamp01 = TilesFactory.instance.InstantiateTile(swampTokenProfile);
        newSwamp01.transform.position = _Tile.transform.position;

        yield return BoardController.C_AddNewTile(newSwamp01, _Tile.indexInBoard-1);
        RemoveSkill(newSwamp01._Profile, GenericSkills.Unmovable);

        TileController newSwamp02 = TilesFactory.instance.InstantiateTile(swampTokenProfile);
        newSwamp02.transform.position = _Tile.transform.position;

        yield return BoardController.C_AddNewTile(newSwamp02, _Tile.indexInBoard +1);
        RemoveSkill(newSwamp02._Profile, GenericSkills.Unmovable);

        AddSkill(this, GenericSkills.Unmovable);
        AddSkill(newSwamp01._Profile, GenericSkills.Unmovable);
        AddSkill(newSwamp02._Profile, GenericSkills.Unmovable);
    }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

    public static void RemoveSkill(TileStateClass profile, GenericSkills skill)
    {
        profile.genericSkills.Add(skill);

    }
    public static void AddSkill(TileStateClass profile, GenericSkills skill)
    {
        profile.genericSkills.Remove(skill);
    }
   public override IEnumerator OnPlayerStepped() 
   { 
        yield return _Tile.AddBaseDamage(DamageAddedOnCross);
        yield return base.OnPlayerStepped();
    }
   public override string GetTooltipText() { return $"{OnEnterInBoard} Spawn 2 SWAMPS around \n {OnCrossed} Increase {MathJ.AddDamage(DamageAddedOnCross)} this tile"; }
}