using System.Collections;
using UnityEngine;

public class Tile_Oca_ForeachOcaGetMoney : Tile_Oca
{
    [SerializeField] int moneyPerOca = 3;
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        int OcasCount = 0;
        foreach (Tile_Base tile in BoardController.TilesList)
        {
            if (tile is Tile_Oca) { OcasCount++; }

            GameController.AddMoney(moneyPerOca * OcasCount);
        }
    }
}
