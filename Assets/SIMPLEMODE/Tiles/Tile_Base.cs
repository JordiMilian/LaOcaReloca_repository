using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public enum Intensity
{
    empty, low, mid, large
}
public enum SecundarySkills
{
    empty, extraMoney, extraDamage
}
public enum TileState
{
    none, InShop, InHand, Dragged, InBoard
}
public enum Rarity
{
    none, Common, Rare, Legendary, Unique
}
public class Tile_Base : MonoBehaviour
{
    [SerializeField] SecundarySkills secundarySkill;
    public TileState tileState = TileState.none;
    public Rarity rarity = Rarity.none;
    public int indexInBoard;
    public int IndexInHand;

    [Header("Color testing")]
    public Color tileColor;
     SpriteRenderer tileTestSprite;
    TextMeshPro TMP_IndexDisplay;

    protected GameController_Simple GameController;
    protected Board_Controller_simple BoardController;
    Camera mainCamera;
   [HideInInspector] public TileMovement tileMovement;
    private void Awake()
    {
        GameController = GameController_Simple.Instance;
        BoardController = Board_Controller_simple.Instance;

        tileTestSprite = GetComponentInChildren<SpriteRenderer>();
        TMP_IndexDisplay = GetComponentInChildren<TextMeshPro>();

        UpdateTileVisuals();

        mainCamera = Camera.main;
        tileMovement = GetComponent<TileMovement>();
    }
    public void CopyData(Tile_Base copyingTile)
    {
        tileColor = copyingTile.tileColor;
        rarity = copyingTile.rarity;
    }
    public void UpdateTileVisuals()
    {
        tileTestSprite = GetComponentInChildren<SpriteRenderer>();
        tileTestSprite.color = tileColor;
        TMP_IndexDisplay.text = indexInBoard.ToString();
    }
    public void FirstAppeareanceAnim()
    {
        float duration = 1;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
    }
   

    public IEnumerator OnPlacedInBoard() { yield break; } //animation only, the logic should be handled by the board_controller
    public IEnumerator OnReplacedInBoard() { yield break; }
    void shakeTile(Intensity intensity)
    {
        switch (intensity)
        {
            case Intensity.empty: break;
            case Intensity.low:
                transform.DOShakeRotation(0.2f, .1f, 1);
                break;
            case Intensity.mid:
                transform.DOShakeRotation(0.4f, .3f, 1);
                break;
            case Intensity.large:
                transform.DOShakeRotation(0.6f, .4f, 1);
                break;
        }
    }
    
    public virtual IEnumerator OnPlayerStepped()
    {
        switch (secundarySkill)
        {
            case SecundarySkills.extraDamage:
                shakeTile(Intensity.low);
                yield return GameController_Simple.Instance.AddAcumulatedDamage(20);
                break;
            case SecundarySkills.extraMoney:
                shakeTile(Intensity.low);
                 GameController_Simple.Instance.AddMoney(5);
                break;
            case SecundarySkills.empty:
                break;
        }
        yield break;

        //INHERITEDS
        //gamelogic
        //visual feedback
        //further logic

    }
    public virtual IEnumerator OnPlayerLanded()
    {
        shakeTile(Intensity.mid);
        yield break;
    }
    public virtual float GetLandedDamageAmount()
    {
        return 0;
    }
    #region BUY/SELL
    public int GetBuyingPrice()
    {
        switch(rarity)
        {
            case Rarity.Common: { return 2; }
            case Rarity.Rare: { return 4; }
            case Rarity.Legendary: { return 8; }
            default: { Debug.LogError($"ERROR: Pls set a valid rarity to this {GetType()}"); return 0; } 
        }
    }
    public int GetSellingPrice()
    {
        switch (rarity)
        {
            case Rarity.Common: { return 1; }
            case Rarity.Rare: { return 2; }
            case Rarity.Legendary: { return 4; }
            default: { Debug.LogError("ERROR: Pls set a valid rarity to this Tile"); return 0; }
        }
    }


    #endregion
}
