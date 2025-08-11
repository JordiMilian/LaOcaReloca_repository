using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ShopController : MonoBehaviour
{
    public List<GameObject> TilesAppeareable;
    [Header("UI")]
    public ShopItem_Controller[] shopItems;
    [SerializeField] int rerollPrice = 5;
    [SerializeField] Button button_Reroll;

    public ShopItem_Controller GetShopItem(Tile_Base tile)
    {
        foreach(ShopItem_Controller shopItem in shopItems)
        {
            if(shopItem.Item == tile)
            {
                return shopItem;
            }
        }
        return null;
    }
    #region DISABLE SHOP
    public void DisableShop()
    {
        
    }
    public void EnableShop()
    {
        foreach (ShopItem_Controller shopItem in shopItems)
        {
            if (shopItem.Item != null) { shopItem.Item.tileMovement.canBeMoved = true; }
        }
        button_Reroll.interactable = true;
    }
    #endregion
    public void Button_ReRollShop()
    {
        GameController_Simple gameController = GameController_Simple.Instance;

        if(gameController.CanPurchase(rerollPrice))
        {
            gameController.RemoveMoney(rerollPrice);
            ResetShopItems();
        }
    }
    public void ResetShopItems() // this is called at start game to create the initial shop too
    {
        foreach (ShopItem_Controller item in shopItems)
        {
            item.ResetShopItem();
        }
    }
    public GameObject getRandomTilePrefab()
    {
        return TilesAppeareable[UnityEngine.Random.Range(0, TilesAppeareable.Count)];
    }

    public void UpdatePrices()
    {
        foreach(ShopItem_Controller item in shopItems)
        {
            if(item.Item != null)
            {
                item.UpdatePriceTag();
            }
        }
    }

}
