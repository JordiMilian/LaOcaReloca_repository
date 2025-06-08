using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ShopController : MonoBehaviour
{
    [SerializeField] GameObject[] TilesAppeareable;
    [Header("UI")]
    [SerializeField] ShopItem_Controller[] shopItems;
    List<Button> shopItemsButtons = new();
    [SerializeField] int rerollPrice = 5;
    [SerializeField] Button button_Reroll;

    private void Awake()
    {
        foreach(ShopItem_Controller item in shopItems)
        {
            shopItemsButtons.Add(item.GetComponentInChildren<Button>());
        }
    }
    #region DISABLE SHOP
    public void DisableShop()
    {
        foreach(Button button in shopItemsButtons) { button.interactable = false; }
        button_Reroll.interactable = false;
    }
    public void EnableShop()
    {
        foreach (Button button in shopItemsButtons) { button.interactable = true; }
        button_Reroll.interactable = false;
    }
    #endregion
    public void Button_ReRollShop()
    {
        GameController_Simple gameController = GameController_Simple.Instance;

        if(gameController.CanPurchase(rerollPrice))
        {
            gameController.Purchase(rerollPrice);
            foreach (ShopItem_Controller item in shopItems)
            {
                item.ResetShopItem();
            }
        }
    }
    public GameObject getRandomTilePrefab()
    {
        return TilesAppeareable[UnityEngine.Random.Range(0, TilesAppeareable.Length)];
    }
  
}
