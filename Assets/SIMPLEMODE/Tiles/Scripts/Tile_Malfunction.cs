using UnityEngine;
using System.Collections;
using static StringTools;
using System.Linq;
public class Tile_Malfunction : TileInfo
{
    [SerializeField] float chanceToTrigger = 0.3f;
    [SerializeField] int limit = 10, spawnedTokens = 0;
    TileController lastTileSpawnedByThis = null;
    public override TileInfo GetCopy()
    {
        Tile_Malfunction newInfo = (Tile_Malfunction)CopyBaseStatsIntoOther(new Tile_Malfunction());
        newInfo.chanceToTrigger = chanceToTrigger;
        newInfo.limit = limit;
        newInfo.spawnedTokens = spawnedTokens;
        newInfo.lastTileSpawnedByThis = lastTileSpawnedByThis;
        return newInfo;
    }
    public override IEnumerator OnPlacedInBoard() 
    {
        yield return base.OnPlacedInBoard();
        GameController.OnAddedNewTileToBoard_CardEffect.AddEffect(OnAddedTile);
    }
   public override IEnumerator OnRemovedFromBoard() 
    { 
        yield return base.OnRemovedFromBoard(); 
        GameController.OnAddedNewTileToBoard_CardEffect.RemoveEffect(OnAddedTile); 
    }
    IEnumerator OnAddedTile(TileController newTile)
    {
        if(spawnedTokens >= limit)
        {
            yield break;
        }
        if (newTile._Info.tileTags.Contains(TileTags.Token))
        {
            if(newTile == lastTileSpawnedByThis) //don't trigger on tiles spawned by this 
            {
                yield break;
            }
            if (passedChance())
            {
                TileController tokenCopy = TilesFactory.instance.InstantiateTileCopy(newTile._Info);
                tokenCopy.transform.position = _Controller.transform.position;
                tokenCopy.SetBaseDamage(0);
                lastTileSpawnedByThis = tokenCopy;

                tileMovement.shakeTile(Intensity.mid);
                yield return BoardController.C_AddNewTile(tokenCopy, _Controller.indexInBoard);
                spawnedTokens++;
            }
        }
    }
    bool passedChance()
    {
        float random = Random.Range(0f, 1f);
        if(random <= chanceToTrigger)
        {
            return true;
        }
        return false;
    }
   //public override IEnumerator OnPlayerLanded() { yield return base.OnPlayerLanded(); }

   public override IEnumerator OnPlayerStepped() { yield return base.OnPlayerStepped(); spawnedTokens = 0; }
   public override string GetTooltipText() { return $"{OnCustomMessaje("WHEN A NEW TOKEN IS SPAWNED")} {chanceToTrigger * 100}% chance to spawn a copy with 0 DMG. Limit {limit}({limit-spawnedTokens})\n"
            +$"{OnCrossed} Restart limit"; }
}