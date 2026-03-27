using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "TileProfile/Money/Pirate", fileName = "Tile_Pirate")]
public class Tile_Pirate : TileInfo
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }

    [SerializeField] int moneyOnLanded = 2;
    [SerializeField] float multiplier = 1;

    public override IEnumerator OnPlayerLanded()
    {
        multiplier++;
        yield return base.OnPlayerLanded(); 
    }
    public override IEnumerator OnTileFinished()
    {
        _Controller.DamagesToDeal.Add(currentMoney() * multiplier);

        return base.OnTileFinished();
    }
    int currentMoney()
    {
        if (GameController != null) { return GameController.GetCurrentMoney(); } else { return 0; }
    }
    public override string GetTooltipText()
    {
        return $"{OnCrossed} Deal damage equal to your CURRENT MONEY {StringTools.ColorText("X" + multiplier.ToString(),"blue")}({currentMoney() * multiplier})" +
            $"\n{OnLanded} Increase that amount +1";
    }

}
