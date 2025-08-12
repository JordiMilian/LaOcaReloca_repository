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
    }
    public void RemoveItem()
    {
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

        UpdatePriceTag();
    }
    public void UpdatePriceTag()
    {
        TMP_Price.text = "$"+Item.GetBuyingPrice().ToString();
    }

}
