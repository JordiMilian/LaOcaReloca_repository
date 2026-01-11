using UnityEngine;

[CreateAssetMenu(menuName = "Buyable Group/Buyables Group - Basic", fileName = "new Buyables Group - Basic")]
public class BuyablesGroup_Basic : BuyablesGroup
{
    public GameObject[] BuyablesList_GO;
    public override GameObject GetRandomBuyableGO()
    {
        return Instantiate(BuyablesList_GO[Random.Range(0,BuyablesList_GO.Length)]); 
    }
    public override int BuyablesCount()
    {
        return BuyablesList_GO.Length;
    }
}
