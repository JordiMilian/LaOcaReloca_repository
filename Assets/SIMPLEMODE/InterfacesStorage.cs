using System;
using System.Collections;
using UnityEngine;

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
    public IEnumerator OnEncounterEnter();
    public IEnumerator OnEncounterExit();
}
public interface ITooltip
{
    public string GetTooltipDescription();
    public string GetTooltipTitle();
    public Texture GetTooltipTexture();
    
}
public enum Intensity
{
    empty, low, mid, large
}
public enum TileState
{
    none, InShop, InBoard
}
public enum Rarity
{
    none, Common, Rare, Legendary, Unique
}
public enum TileTags
{
    NoTag, Empty, Oca, Rat, Token
}
public enum TileSize
{
    Small, Medium, Large
}
public enum TileMessageType
{
    Neutral, DealDamage, AddMultiplier, AddBaseDamage
}
