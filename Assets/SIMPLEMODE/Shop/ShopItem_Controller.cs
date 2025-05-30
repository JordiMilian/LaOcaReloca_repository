using TMPro;
using UnityEngine;

public class ShopItem_Controller : MonoBehaviour
{
    public Tile_Base Item;
    public GameObject TileGO;
    [SerializeField] Transform tilePrefabTf;
    GameController_Simple gameController;
    [SerializeField] ShopController shopController;
    [SerializeField] TextMeshProUGUI TMP_Price;

    private void Start()
    {
        gameController = GameController_Simple.Instance;
        ResetShopItem();
    }

    public void Button_OnBuyPressed()
    {
        if (!gameController.CanPurchase(Item.GetBuyingPrice())) { return; }
        if (gameController.isHandFull()) { return; }
        gameController.Purchase(Item.GetBuyingPrice());
        gameController.AddTileToHand(TileGO);

        ResetShopItem();
    }
    void ResetShopItem()
    {

        TileGO = gameController.InstantiateNewTile(shopController.getRandomAppearable());
        Item = TileGO.GetComponent<Tile_Base>();
        TileGO.transform.position = tilePrefabTf.position;
        TileGO.transform.rotation = tilePrefabTf.rotation;

        TMP_Price.text = Item.GetBuyingPrice().ToString();

        Item.tileMovement.canBeDragged = false;

    }
}
