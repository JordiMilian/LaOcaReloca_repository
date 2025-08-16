using System;

public class InterfacesStorage
{
}
public interface IBuyable
{
    public int GetBuyingPrice();
    public Action<ShopItem_Controller> OnAppearInShop { get; set; }
}
