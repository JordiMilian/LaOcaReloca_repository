using UnityEngine;
using System.Collections;
public class Tile_DobleEmpty : TileInfo
{
   [SerializeField] TileConfig emptyProfile;

   //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   public override IEnumerator OnPlayerLanded() 
   { 
        yield return base.OnPlayerLanded();
        TileController instantiatedEmpty =  TilesFactory.instance.InstantiateTile(emptyProfile._configInfo);
        instantiatedEmpty.transform.position = _Controller.transform.position;
        int randomIndex = Random.Range(1, BoardController.TilesList.Count - 1);
        instantiatedEmpty.SetBaseDamage(BaseDamage);
        yield return BoardController.C_AddNewTile(instantiatedEmpty, randomIndex);
        

   }
    //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
    public override TileInfo GetCopy()
    {
        Tile_DobleEmpty newInfo = (Tile_DobleEmpty)CopyBaseStatsIntoOther(new Tile_DobleEmpty());
        newInfo.emptyProfile = emptyProfile;
        return newInfo;
    }
    public override string GetTooltipText() { return $"{OnLanded} Create a random Empty Tile with this tile damage({BaseDamage})"; }
}