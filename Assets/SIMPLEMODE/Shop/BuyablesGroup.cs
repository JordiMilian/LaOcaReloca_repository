using UnityEngine;


public abstract class BuyablesGroup : ScriptableObject
{
    public float ChangeToAppear = 1;

    public abstract GameObject GetRandomBuyableGO();
    
}

