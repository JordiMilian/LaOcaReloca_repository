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
        if (TileGO == null) { return; } //item already bought, empty shop
        if (gameController.isHandFull()) { return; } //hand full
        if (!gameController.CanPurchase(Item.GetBuyingPrice())) { return; } //no money
        
        

        gameController.Purchase(Item.GetBuyingPrice());
        gameController.AddTileToHand(TileGO);

        Item = null;
        TileGO = null;

        TMP_Price.text = "";

    }
    public void ResetShopItem()
    {
        if(TileGO != null) { Destroy(TileGO); }

        TileGO = Instantiate(shopController.getRandomTilePrefab(), shopController.transform);
        Item = TileGO.GetComponent<Tile_Base>();
        Item.tileMovement.SetOriginTransformWithTransform(tilePrefabTf);
        Item.tileMovement.PlaceTileInOrigin();
        Item.SetTileState(TileState.InShop);

        TMP_Price.text = Item.GetBuyingPrice().ToString();

    }
}
