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
        //ResetShopItem();
    }


    public void Button_OnBuyPressed()
    {
        if (!gameController.CanPurchase(Price)) { return; }
        if (gameController.isHandFull()) { return; }
        gameController.AddItemToHand(TileGO);
        gameController.AttemptPurchase(Price);

        //ResetShopItem();
    }
    void ResetShopItem()
    {
        if(TileGO != null) { Destroy(TileGO); }

        TileGO = gameController.InstantiateNewTile(shopController.getRandomAppearable());
        Item = TileGO.GetComponent<Tile_Base>();
        TileGO.transform.position = tilePrefabTf.position;
        TileGO.transform.rotation = tilePrefabTf.rotation;
    }
}
