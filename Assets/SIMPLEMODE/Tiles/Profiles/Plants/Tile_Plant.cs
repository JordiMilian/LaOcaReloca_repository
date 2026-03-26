using System;
using System.Collections;
using UnityEngine;

public abstract class Tile_Plant : TileStateClass
{
    public override IEnumerator OnPlacedInBoard()
    {
        yield return base.OnPlacedInBoard();

        if(PlantsManager.Instance == null)
        { PlantsManager.Instance = new PlantsManager(); PlantsManager.Instance.Initialize(); }

        PlantsManager.Instance.PlantsGrowthCoroutine.AddCoroutine(C_growPlant);
        growthModifiers = null;
    }
    public override IEnumerator OnRemovedFromBoard()
    {
        yield return base.OnRemovedFromBoard();

        PlantsManager.Instance.PlantsGrowthCoroutine.RemoveCoroutine(C_growPlant);
        //GameController.OnRolledDice_CardEffects.RemoveEffect(C_growPlant);
    }

    public float baseGrowth = 1f;

    public Func<float, float> growthModifiers;

    IEnumerator C_growPlant()
    {
        Debug.Log("plants 02.25 grow plant");
        yield return _Tile.AddBaseDamage(GetFinalGrowth());
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
    public override string GetTooltipText() { return StringTools.Growth(GetFinalGrowth()); }

}

public class PlantsManager
{
    public static PlantsManager Instance;
    public SImultaneousCoroutine PlantsGrowthCoroutine;

    public void Initialize()
    {
        PlantsGrowthCoroutine = new SImultaneousCoroutine();
        GameController_Simple.Instance.OnRolledDice_CardEffects.AddEffect(PlantsGrowthCoroutine.C_ExecuteCoroutines);
    }

}
