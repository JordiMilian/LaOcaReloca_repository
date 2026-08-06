using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
public class Tile_SwampMonster : TileInfo
{
    [SerializeField] TileConfig swampTokenProfile;
    [SerializeField] float DamageAddedOnCross = 100;
    public override IEnumerator OnPlacedInBoard() 
   {
        yield return base.OnPlacedInBoard();
        RemoveSkill(this, GenericSkills.Unmovable);

        TileController newSwamp01 = TilesFactory.instance.InstantiateTileFromConfig(swampTokenProfile);
        newSwamp01.transform.position = _Controller.transform.position;

        yield return BoardController.C_AddNewTile(newSwamp01, _Controller.indexInBoard-1);
        RemoveSkill(newSwamp01._Info, GenericSkills.Unmovable);

        TileController newSwamp02 = TilesFactory.instance.InstantiateTileFromConfig(swampTokenProfile);
        newSwamp02.transform.position = _Controller.transform.position;

        yield return BoardController.C_AddNewTile(newSwamp02, _Controller.indexInBoard +1);
        RemoveSkill(newSwamp02._Info, GenericSkills.Unmovable);

        AddSkill(this, GenericSkills.Unmovable);
        AddSkill(newSwamp01._Info, GenericSkills.Unmovable);
        AddSkill(newSwamp02._Info, GenericSkills.Unmovable);
    }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
    //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

    public static void RemoveSkill(TileInfo profile, GenericSkills skill)
    {
        profile.genericSkills.Add(skill);

    }
    public static void AddSkill(TileInfo profile, GenericSkills skill)
    {
        profile.genericSkills.Remove(skill);
    }
   public override IEnumerator OnPlayerStepped() 
   { 
        yield return _Controller.AddBaseDamage(DamageAddedOnCross);
        yield return base.OnPlayerStepped();
    }
   public override string GetTooltipText() { return $"{OnEnterInBoard} Spawn 2 SWAMPS around \n {OnCrossed} Increase {MathJ.AddDamage(DamageAddedOnCross)} this tile"; }
    public override TileInfo GetCopy()
    {
        Tile_SwampMonster newInfo = (Tile_SwampMonster)CopyBaseStatsIntoOther(new Tile_SwampMonster());
        newInfo.DamageAddedOnCross = DamageAddedOnCross;
        newInfo.swampTokenProfile = swampTokenProfile;
        return newInfo;
    }

}