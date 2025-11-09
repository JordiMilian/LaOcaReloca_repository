using UnityEngine;
using System.Collections;
using static StringTools;
using System.Linq;
public class Tile_Malfunction : Tile_Profile
{
    [SerializeField] float chanceToTrigger = 0.3f;
    [SerializeField] int limit = 10, spawnedTokens = 0;
    TileController lastTileSpawnedByThis = null;
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
        if (newTile._Profile.tileTags.Contains(TileTags.Token))
        {
            if(newTile == lastTileSpawnedByThis) //don't trigger on tiles spawned by this 
            {
                yield break;
            }
            if (passedChance())
            {
                TileController tokenCopy = TilesFactory.instance.InstantiateTile(newTile._Profile);
                tokenCopy.transform.position = _Tile.transform.position;
                tokenCopy.SetBaseDamage(0);
                lastTileSpawnedByThis = tokenCopy;

                tileMovement.shakeTile(Intensity.mid);
                yield return BoardController.C_AddNewTile(tokenCopy, _Tile.indexInBoard);
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