using UnityEngine;
using System.Collections;
using static StringTools;
public class Tile_SpidersNest : Tile_Profile
{
    [SerializeField] int spidersOnCrossed = 1, spidersOnRolledDice = 1;
    [SerializeField] Tile_Profile spiderTokenTileProfile;
    public override IEnumerator OnPlacedInBoard() { yield return base.OnPlacedInBoard(); GameController.OnRolledDice_CardEffects.AddEffect(SpawnRolledDiceSpiders); }
    public override IEnumerator OnRemovedFromBoard() { yield return base.OnRemovedFromBoard(); GameController.OnRolledDice_CardEffects.RemoveEffect(SpawnRolledDiceSpiders); }
    public override IEnumerator OnPlayerStepped() 
    { 
        yield return base.OnPlayerStepped();
        for (int i = 0; i < spidersOnCrossed; i++)
        {
            yield return CreateRandomSpider();
        }
    }
    IEnumerator SpawnRolledDiceSpiders()
    {
        for (int i = 0; i < spidersOnRolledDice; i++)
        {
            yield return CreateRandomSpider();
        }
    }

    IEnumerator CreateRandomSpider()
    {
        TileController ratTokenController = TilesFactory.instance.InstantiateTile(spiderTokenTileProfile);
        ratTokenController.transform.position = _Tile.transform.position;

        int randomIndex = MathJ.GetRandomIndexInBoard(true);
        yield return BoardController.C_AddNewTile(ratTokenController, randomIndex);
    }
    public override string GetTooltipText()
    {
        return $"{OnRolledDice} Spawn {spidersOnRolledDice} spiders. \n{OnCrossed} Spawn {spidersOnCrossed} spiders.";
    }
}