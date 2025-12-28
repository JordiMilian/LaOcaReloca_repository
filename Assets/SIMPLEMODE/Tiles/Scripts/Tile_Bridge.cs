using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_Bridge : Tile_Profile
{
   [HideInInspector] public Tile_Bridge otherBridge;
   [HideInInspector] public bool ignoreSpawnCopy = false;
   public override IEnumerator OnPlacedInBoard() 
    { 
        yield return base.OnPlacedInBoard();
        if (ignoreSpawnCopy) { yield break; }

        TileController bridgeCopy = TilesFactory.instance.InstantiateTile(this);
        otherBridge = bridgeCopy._Profile as Tile_Bridge;
        otherBridge.otherBridge = this;
        otherBridge.ignoreSpawnCopy = true;
        bridgeCopy.transform.position = _Tile.transform.position;

        yield return BoardController.C_AddNewTile(bridgeCopy, _Tile.indexInBoard +1);

        otherBridge.ignoreSpawnCopy = false;
    }
   public override IEnumerator OnRemovedFromBoard() 
    { 
        yield return base.OnRemovedFromBoard();
        if(otherBridge != null)
        {
            otherBridge.otherBridge = null;
        }
    }
   public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        yield return _Tile.C_DealAllDamageToDeal();
        if(otherBridge !=null)
        {
            yield return BoardController.L_JumpPlayerTo(otherBridge._Tile.indexInBoard, false);
        }
        
    }
   //public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); }
   public override string GetTooltipText() { return $"{StringTools.OnEnterInBoard} Creates a copy of itself.\n{OnLanded} Jump to the other Bridge."; }
}