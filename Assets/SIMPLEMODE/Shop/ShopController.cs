using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Security.Cryptography;

public class ShopController : MonoBehaviour
{
    [SerializeField] BuyablesGroup[] buyableGroups;
    [Header("UI")]
    public ShopItem_Controller[] shopItems;
    [SerializeField] int baseRerollPrice = 5;
    int currentRerollPrice;
    [SerializeField] Button button_Reroll;
    bool shopEnabled = true;
    [SerializeField] TextMeshProUGUI TMP_buttonText;

    public ShopItem_Controller GetShopItem(IBuyable buyable)
    {
        foreach(ShopItem_Controller shopItem in shopItems)
        {
            if(shopItem.buyable == buyable)
            {
                return shopItem;
            }
        }
        return null;
    }
    #region DISABLE SHOP
    public void DisableShop()
    {
        foreach (ShopItem_Controller shopItem in shopItems)
        {
            if (shopItem.buyable != null) { shopItem.buyable.OnDisablePurchase(); }
        }
        shopEnabled = false;
    }
    public void EnableShop()
    {
        foreach (ShopItem_Controller shopItem in shopItems)
        {
            if (shopItem.buyable != null) { shopItem.buyable.OnEnablePurchase(); }
        }
        shopEnabled = true;
    }
    #endregion
    private void Start()
    {
        GameController_Simple.Instance.OnKilledEnemy.AddListener(ResetRerollPrice);
        ResetRerollPrice();
    }
    void ResetRerollPrice()
    {
        currentRerollPrice = baseRerollPrice;
        UpdateRerollPriceDisplay();
    }
    void UpdateRerollPriceDisplay()
    {
        TMP_buttonText.text = $"Reroll -> {currentRerollPrice}$";
    }
    public void Button_ReRollShop()
    {
        GameController_Simple gameController = GameController_Simple.Instance;

        if(gameController.CanPurchase(currentRerollPrice))
        {
            gameController.RemoveMoney(currentRerollPrice);
            currentRerollPrice++;
            UpdateRerollPriceDisplay();    

            ResetAllShopItems();
            if(shopEnabled == false)
            {
                foreach (ShopItem_Controller item in shopItems)
                {
                    item.buyable.OnDisablePurchase();
                }
            }
        }
    }
    public void ResetAllShopItems() // this is called at start game to create the initial shop too
    {
        foreach (ShopItem_Controller item in shopItems)
        {
            item.ResetShopItem();
        }
    }
    public GameObject GetRandomBuyableGO()
    {
        float totalChance = 0;
        //Add up all the chances
        foreach (BuyablesGroup group in buyableGroups)
        {
            totalChance += group.ChanceToAppear;
        }

        float randomChance = Random.Range(0, totalChance);
        float counting = 0;
        foreach (BuyablesGroup group in buyableGroups)
        {
            float prev = counting;
            counting += group.ChanceToAppear;
            if (randomChance <= counting && randomChance > prev)
            {
                return group.GetRandomBuyableGO();
            }
        }
        return null;

    }
    public void UpdateBuyablesChangePercent()
    {
        float total = 0;
        foreach(BuyablesGroup buyable in buyableGroups) { total += buyable.ChanceToAppear;}
        foreach(BuyablesGroup buyable in buyableGroups)
        {
            float percent = 100 * buyable.ChanceToAppear / total;
            buyable.ChanceToAppear_PerCent = percent;
            buyable.ChancePerElement_PerCent = percent / buyable.BuyablesCount();
        }
    }

    public void UpdatePrices()
    {
        foreach(ShopItem_Controller item in shopItems)
        {
            if(item.buyable != null)
            {
                item.UpdatePriceTag();
            }
        }
    }

}
