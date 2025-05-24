using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Situacio ideal crear cartes noves:

Tile ScriptablesObject
float PriceInShop (maybe depend on rarity)

Enum: Rarity

Enum: Trigger (onLanded, OnStepped, whenPlaced, whenGainMoney, whenReachEnd, whenReachBegin, whenOcaJump, whenRollDice)

void override IEnumerator MainEffect()


Enum SecundaryTrigger(none, OnLanded, OnStepped)
void override IEnumerator secundarySkill()

TileInstance_Data
 */
public class Tile_Controller : MonoBehaviour
{
    public Tile_Data data { get; private set; }
    public Tile_View view { get; private set; }
    public void InitializeTile(Tile_Data tileData)
    {
        data = tileData;
        view = GetComponent<Tile_View>();
    }
    public IEnumerator OnPlayerStepped()
    {
        yield return view.OnPlayerStepped();
     
        yield return data.OnPlayerStepped_logic();
    }
    public IEnumerator TriggerMainEffect()
    {
        yield return view.OnPlayerLanded();
        yield return data.OnPlayerLanded_logic();
    }
    public IEnumerator AddDamage()
    {
        yield return GameControlle.Instance.AddAcumulatedDamage(data.GetDamageAmount());
    }
    public void AppearTile()
    {
        view.TileAppear();
    }
}
