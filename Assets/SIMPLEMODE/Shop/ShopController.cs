using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ShopController : MonoBehaviour
{
    public class ShopItem
    {
        public Tile_Base Item;
        public GameObject TileGO;
        public int Price;
    }
    [SerializeField] List<Tile_Base> TilesAppeareableInShop = new();
    public List<ShopItem> ShopItemsList;
    [SerializeField] int maxItemsInShop;
    [Header("UI")]
    [SerializeField] Button BuyItem01;
    [SerializeField] Button BuyItem02;
    [SerializeField] Button BuyItem03;
    [SerializeField] TextMeshProUGUI ItemName_01, ItemName_02, ItemName_03;
    [SerializeField] TextMeshProUGUI ItemPrice_01, ItemPrice_02, ItemPrice_03;
    [SerializeField] Transform ItemPos01, ItemPos02, ItemPos03;


    const int placeholderPrice = 10;
    private void Start()
    {
       /*
        FillShop();
        UpdateUI();
        //
        void FillShop()
        {
            ShopItemsList = new();
            for (int i = 0; i < maxItemsInShop; i++)
            {
                ShopItem item = new ShopItem();
                item.Item = getRandomAppearable();
                item.Price = placeholderPrice;
                item.TileGO = GameController_Simple.Instance.InstantiateNewTile(item.Item);

                ShopItemsList.Add(item);
            }
        }
       */
    }
    #region DISABLE SHOP
    public void DisableShop()
    {
        BuyItem01.interactable = false;
        BuyItem02.interactable = false;
        BuyItem03.interactable = false;
    }
    public void EnableShop()
    {
        BuyItem01.interactable = true;
        BuyItem02.interactable = true;
        BuyItem03.interactable = true;
    }
    #endregion
    void UpdateUI()
    {
        ItemName_01.text = ShopItemsList[0].Item.name;
        ItemName_02.text = ShopItemsList[1].Item.name;
        ItemName_03.text = ShopItemsList[2].Item.name;

        ItemPrice_01.text = placeholderPrice.ToString();
        ItemPrice_02.text = placeholderPrice.ToString();
        ItemPrice_03.text = placeholderPrice.ToString();
    }

    
    public Tile_Base getRandomAppearable()
    {
        return TilesAppeareableInShop[Random.Range(0, TilesAppeareableInShop.Count)];
    }
  
}
