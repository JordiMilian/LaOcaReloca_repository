using UnityEngine;
using System.Collections;

public class Tile_Profile : ScriptableObject
{
    public float BaseDamage = 10;
    public string Title = "NO TITLE";
    public Color tileColor = Color.gray;
    public Texture tileTexture;
    [HideInInspector] public Rarity rarity = Rarity.none;
    [HideInInspector] public int uniquePrice = 0; //IF rarity is Unique, use this value.
    public TileTags tileTag;
    [HideInInspector] public TileController _Tile;
    protected TileSharedVisuals tileMovement;
    [Space(5)]
    protected Board_Controller_simple BoardController;
    protected GameController_Simple GameController;
    public bool isMicroTile = false;
    
    public void Initialize()
    {
        BoardController = Board_Controller_simple.Instance;
        GameController = GameController_Simple.Instance;
        tileMovement = _Tile.tileMovement;
    }
    public virtual IEnumerator OnPlayerStepped()
    {
        _Tile.DamagesToDeal.Add(BaseDamage);
        _Tile.DamagesToDeal.Reverse();

        yield return GameController.OnCrossed_CardEffects.C_ActivateEffects();

        if (isMicroTile ||  GameController_Simple.Instance.remainingStepsToTake != 1)//if it's the last tile to step before landing, we don't deal the damage because it will be dealt all toghere on landing
        {
            yield return _Tile.C_DealAllDamageToDeal(); 
        }
    }
    public virtual IEnumerator OnPlayerLanded() 
    {
        yield return _Tile.C_DealAllDamageToDeal();
    }
    public virtual void OnPlacedInBoard() { }
    public virtual void OnRemovedFromBoard() {}
    public virtual string GetTooltipText() { return "NO DESCRIPTION FOUND"; }
    #region TOOLTIP INTRO
    protected const string OnCrossed = "<b>- ON CROSSED:</b>";
    protected const string OnLanded = "<b>- ON LANDED:</b>";
    protected const string OnRolledDice = "<b>- ON ROLLED DICES:</b>";
    protected const string OnReachedEnd = "<b>- ON REACHED END TILE:</b>";
    protected const string OnReached = "<b>- ON REACHED:</b>";
    protected const string OnAddedDamage = "<b>- ON ADDED DAMAGE TO THIS TILE:</b>";
    protected string OnLandedOnTag(TileTags tag) { return $"<b>- ON LANDED ON AN {tag.ToString().ToUpper()} TILE:</b>"; }
    protected string OnCrossedOnTag(TileTags tag) { return $"<b>- ON CROSSED A {tag.ToString().ToUpper()} TILE:</b>"; }
    #endregion

    #region DAMAGE MODIFIERS 
    //Separated the logic of modifying in case we want to Override the logic and not the visuals
    //DO NOT CALL THESE FROM THE TILE LOGIC, THIS IS FOR OVERRIDING ONLY (Look at Tile_Creating for a good examples)
    //IF YOU WANT TO MULTIPLY DAMAGE CALL IT FROM Tile.MultiplyBaseDamage()
    public virtual float AddBaseDamage(float addedDamage)
    {
        _Tile.SetBaseDamage(BaseDamage + addedDamage);
        return addedDamage;
    }
    public virtual float RemoveBaseDamage(float removedDamage)
    {
        if (removedDamage > BaseDamage)
        {
            removedDamage = BaseDamage;
        }
        _Tile.SetBaseDamage(BaseDamage - removedDamage);
        return removedDamage;
    }
    public virtual void MultiplyBaseDamage(float mult)
    {
        _Tile.SetBaseDamage(BaseDamage * mult);
    }
    #endregion
}
