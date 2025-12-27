using System;
using System.Collections;
using UnityEngine;

public abstract class Tile_Plant : Tile_Profile
{
    public override IEnumerator OnPlacedInBoard()
    {
        yield return base.OnPlacedInBoard();
        GameController.OnRolledDice_CardEffects.AddEffect(C_growPlant);
        growthModifiers = null;
    }
    public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();
        GameController.OnRolledDice_CardEffects.RemoveEffect(C_growPlant);
    }

    public float baseGrowth = 1f;

    public Func<float, float> growthModifiers;

    IEnumerator C_growPlant()
    {
        _Tile.AddBaseDamage(GetFinalGrowth());
        yield break;
    }
    float GetFinalGrowth()
    {
        float growth = baseGrowth;
        if (growthModifiers != null)
        {
            foreach (Func<float, float> modifier in growthModifiers.GetInvocationList())
            {
                growth = modifier(growth);
            }
        }
        return growth;
    }
    public override string GetTooltipText() { return $"{StringTools.Growth(GetFinalGrowth())}"; }

}
