using UnityEngine;
using System.Collections;
using static StringTools;
using System.Linq;
public class Toy_Malfunction : Toy_Profile
{
    [SerializeField] float chanceToTrigger = 0.3f;
    TileController lastTileSpawnedByThis = null;
    public override void OnActivatedToy()
    {
        _gameController.OnAddedNewTileToBoard_CardEffect.AddEffect(OnAddedTile);
    }
   public override void OnDeactivatedToy()
    {
        _gameController.OnAddedNewTileToBoard_CardEffect.RemoveEffect(OnAddedTile);
    }
    IEnumerator OnAddedTile(TileController newTile)
    {

        if (newTile._Profile.tileTags.Contains(TileTags.Token)&& newTile.GetBaseDamage() > 0)
        {
            if (newTile == lastTileSpawnedByThis) //don't trigger on tiles spawned by this 
            {
                yield break;
            }
            if (passedChance())
            {
                TileController tokenCopy = TilesFactory.instance.InstantiateTile(newTile._Profile);
                tokenCopy.transform.position = _ToyController.transform.position;
                tokenCopy.SetBaseDamage(0);
                lastTileSpawnedByThis = tokenCopy;

                yield return _boardController.C_AddNewTile(tokenCopy, Random.Range(1,_boardController.TilesList.Count -2));
            }
        }

        bool passedChance()
        {
            float random = Random.Range(0f, 1f);
            if (random <= chanceToTrigger)
            {
                return true;
            }
            return false;
        }
    }
    public override string GetTooltipDescription() 
    {
        return $" {OnCustomMessaje("WHEN A NEW TOKEN IS SPAWNED")} If token DMG > 0,  {chanceToTrigger * 100}% chance to spawn a copy with 0 DMG.";
    }
}

