using TMPro;
using UnityEngine;

public class ShopItem_Controller : MonoBehaviour
{
    public IBuyable buyable;
    public GameObject buyableGO;
    public Transform buyablePositionTf;
    [SerializeField] ShopController shopController;
    [SerializeField] TextMeshProUGUI TMP_Price;
    
    public void RemoveItem()
    {
        buyable = null;
        buyableGO = null;
        TMP_Price.text = "";
    }
    public void ResetShopItem()
    {
        if(buyableGO != null) { Destroy(buyableGO); }

        buyableGO = Instantiate(shopController.getRandomBuyable(), shopController.transform);
        buyable = buyableGO.GetComponent<IBuyable>();
        buyable.OnAppearInShop(this);

        UpdatePriceTag();
    }
    public void UpdatePriceTag()
    {
        TMP_Price.text = "$"+buyable.GetBuyingPrice().ToString();
    }

}
