using UnityEngine;


public abstract class BuyablesGroup : ScriptableObject
{
    public float ChanceToAppear = 1;
    public float ChanceToAppear_PerCent;
    public float ChancePerElement_PerCent;
    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            GameController_Simple.Instance.shopController.UpdateBuyablesChangePercent();
        }
        
    }
    public abstract GameObject GetRandomBuyableGO();
    public abstract int BuyablesCount();
    
}

