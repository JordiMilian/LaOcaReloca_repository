using UnityEngine;

public class BuyablesGroup : ScriptableObject
{
    public string GroupName;
    public float ChangeToAppear = 1;
    public GameObject[] BuyablesList_GO;
    public GameObject GetRandomBuyable()
    {
        return BuyablesList_GO[Random.Range(0, BuyablesList_GO.Length)];
    }
}
