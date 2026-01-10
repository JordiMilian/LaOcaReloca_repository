using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Buyable Group/Buyables Group - Tiles", fileName = "new Buyables Group - Tiles")]
public class BuyablesGroup_Tiles : BuyablesGroup
{
    [SerializeField] ProfilesGroup profilesGroup;
    public override GameObject GetRandomBuyableGO()
    {
       return TilesFactory.instance.InstantiateTile(profilesGroup.GetRandomProfile()).gameObject;
    }
}
