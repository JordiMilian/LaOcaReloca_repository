using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(menuName = "Buyable Group/Buyables Group - Toys", fileName = "new Buyables Group - Toys")]
public class BuyablesGroup_Toys : BuyablesGroup
{
    public override int BuyablesCount()
    {
        return ConfigsDatabase.configsDictionary.Count;
    }

    public override GameObject GetRandomBuyableGO()
    {
        ToysManager manager = ToysManager.Instance;

        return manager.InstantiateToyFromConfig(ConfigsDatabase.GetToyConfig_Random()).gameObject;
    }

}
