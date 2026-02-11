using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "TileProfile/DamageAdders/Matrioska", fileName = "Tile_Matrioska")]
public class Tile_Matrioska : Tile_Profile
{
    [SerializeField] int timesCrossed;
    [SerializeField] int timesNeededToCross = 3;
    [SerializeField] float multiplyCurrentDamageBy = 3;
    public override IEnumerator OnPlayerStepped()
   {
        yield return base.OnPlayerStepped();

        timesCrossed++;
        yield return _Tile.C_MultiplyBaseDamage(multiplyCurrentDamageBy);
        

        if (timesCrossed >= timesNeededToCross)
        {
            yield return _Tile.C_DealAllDamageToDeal();
            yield return BoardController.C_RemoveTile(_Tile.indexInBoard);
        }
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Multiply this TILE dmg by x{multiplyCurrentDamageBy} \nDestroyed after crossing {timesNeededToCross}({timesNeededToCross - timesCrossed}) times";
    }
}
