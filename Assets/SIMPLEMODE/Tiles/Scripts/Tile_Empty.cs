using UnityEngine;

public class Tile_Empty : Tile_Base
{
    [SerializeField] int BuyingPrice = 1;
    public override int GetBuyingPrice()
    {
        return BuyingPrice;
    }
}
