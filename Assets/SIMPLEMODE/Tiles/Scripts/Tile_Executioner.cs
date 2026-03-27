using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "TileProfile/Executioner", fileName = "Tile_Executioner")]
public class Tile_Executioner : TileInfo
{
    //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
    //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }

    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override string GetTooltipText() 
    { return $"{OnLanded} Give {StringTools.Fragile} to another random tile"; }

    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();

        if (BoardController.TilesList.Count == 3) { yield break; } //if its just this tile + Start + End

        TileController randomTile = MathJ.GetRandomTileInBoard(_Controller, true); ;

        randomTile._Profile.genericSkills.Add(GenericSkills.Fragile);
        randomTile.tileMovement.shakeTile(Intensity.mid);
        
    }

}