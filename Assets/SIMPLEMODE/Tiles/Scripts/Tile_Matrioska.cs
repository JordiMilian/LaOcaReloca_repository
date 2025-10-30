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
        timesCrossed++;
        
        _Tile.MultiplyBaseDamage(multiplyCurrentDamageBy);
        yield return base.OnPlayerStepped();

        if (timesCrossed >= timesNeededToCross)
        {
            yield return new WaitForSeconds(.5f);
            BoardController.RemoveTile(_Tile.indexInBoard);
            yield break;
        }
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Multiply this TILE dmg by x{multiplyCurrentDamageBy} \nDestroyed after crossing {timesNeededToCross}({timesNeededToCross - timesCrossed}) times";
    }
}
