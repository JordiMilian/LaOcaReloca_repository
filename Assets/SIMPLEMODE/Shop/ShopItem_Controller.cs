using UnityEngine;

public class ShopItem_Controller : MonoBehaviour
{
    public Tile_Base Item;
    public GameObject TileGO;
    public int Price = 10;
    [SerializeField] Transform tilePrefabTf;
    GameController_Simple gameController;
    [SerializeField] ShopController shopController;

    private void Awake()
    {
        
        
    }
    private void Start()
    {
        gameController = GameController_Simple.Instance;
        ResetShopItem();
    }

    public void Button_OnBuyPressed()
    {
        if (!gameController.CanPurchase(Price)) { return; }
        if (gameController.isHandFull()) { return; }

        ResetShopItem();
    }
    void ResetShopItem()
    {
        Item = shopController.getRandomAppearable();
        if(TileGO != null) { Destroy(TileGO); }
        TileGO = gameController.InstantiateNewTile(Item);
        TileGO.transform.position = tilePrefabTf.position;
        TileGO.transform.rotation = tilePrefabTf.rotation;
    }
}
