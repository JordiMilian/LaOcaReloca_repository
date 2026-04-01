using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/DamageAdders/Matrioska", fileName = "Tile_Matrioska")]
public class Tile_Matrioska : TileInfo
{
    [SerializeField] int timesCrossed;
    [SerializeField] int timesNeededToCross = 3;
    [SerializeField] float multiplyCurrentDamageBy = 3;
    public override IEnumerator OnPlayerStepped()
   {
        yield return base.OnPlayerStepped();

        timesCrossed++;
        yield return _Controller.C_MultiplyBaseDamage(multiplyCurrentDamageBy);
        

        if (timesCrossed >= timesNeededToCross)
        {
            yield return _Controller.C_DealAllDamageToDeal();
            yield return BoardController.C_RemoveTile(_Controller.indexInBoard);
        }
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Multiply this TILE dmg by x{multiplyCurrentDamageBy} \nDestroyed after crossing {timesNeededToCross}({timesNeededToCross - timesCrossed}) times";
    }
    public override TileInfo GetCopy()
    {
        Tile_Matrioska newTile = (Tile_Matrioska)CopyBaseStatsIntoOther(new Tile_Matrioska());
        newTile.timesCrossed = timesCrossed;
        newTile.timesNeededToCross = timesNeededToCross;
        newTile.multiplyCurrentDamageBy = multiplyCurrentDamageBy;
        return newTile;
    }
}
