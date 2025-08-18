using System;
using System.Collections;

public class InterfacesStorage
{
}
public interface IBuyable
{
    public int GetBuyingPrice();
    public void OnAppearInShop(ShopItem_Controller shopItemController);
    public void OnEnablePurchase();
    public void OnDisablePurchase();
   
}
public interface IEncounter
{
    public void OnEncounterEnter();
    public void OnEncounterExit();
}
