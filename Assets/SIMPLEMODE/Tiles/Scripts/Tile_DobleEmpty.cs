using UnityEngine;
using System.Collections;
public class Tile_DobleEmpty : TileStateClass
{
   [SerializeField] TileStateClass emptyProfile;

   //public override void OnPlacedInBoard() { base.OnPlacedInBoard(); }
   //public override void OnRemovedFromBoard() { base.OnRemovedFromBoard(); }
   public override IEnumerator OnPlayerLanded() 
   { 
        yield return base.OnPlayerLanded();
        TileController instantiatedEmpty =  TilesFactory.instance.InstantiateTile(emptyProfile);
        instantiatedEmpty.transform.position = _Tile.transform.position;
        int randomIndex = Random.Range(1, BoardController.TilesList.Count - 1);
        yield return BoardController.C_AddNewTile(instantiatedEmpty, randomIndex);
        instantiatedEmpty.SetBaseDamage(BaseDamage);

   }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{OnLanded} Create a random Empty Tile with this tile damage({BaseDamage})"; }
}