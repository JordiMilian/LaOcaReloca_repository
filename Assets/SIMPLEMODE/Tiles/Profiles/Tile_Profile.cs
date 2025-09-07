using UnityEngine;
using System.Collections;

public class Tile_Profile : ScriptableObject
{
    public float BaseDamage = 10;
    public string Title = "NO TITLE";
    public Color tileColor = Color.gray;
    public Rarity rarity = Rarity.none;
    public int uniquePrice = 0; //IF rarity is Unique, use this value. Would be nice to hide this in Editor
    public TileTags tileTag;
    [HideInInspector] public TileController Tile;
    protected TileSharedVisuals tileMovement;

    protected Board_Controller_simple BoardController;
    protected GameController_Simple GameController;
    public void Initialize()
    {
        BoardController = Board_Controller_simple.Instance;
        GameController = GameController_Simple.Instance;
        tileMovement = Tile.tileMovement;
    }
    public virtual IEnumerator OnPlayerStepped()
    {
        Tile.DamagesToDeal.Add(BaseDamage);
        Tile.DamagesToDeal.Reverse();

        if (GameController_Simple.Instance.remainingStepsToTake != 1) { yield return Tile.C_DealAllDamageToDeal(); }
    }
    public virtual IEnumerator OnPlayerLanded() 
    {
        yield return Tile.C_DealAllDamageToDeal();
    }
    public virtual void OnPlacedInBoard() { }
    public virtual void OnRemovedFromBoard() { }
    public virtual string GetTooltipText() { return "NO DESCRIPTION FOUND"; }
    #region TOOLTIP INTRO
    protected const string OnCrossed = "<b>- ON CROSSED:</b>";
    protected const string OnLanded = "<b>- ON LANDED:</b>";
    protected const string OnRolledDice = "<b>- ON ROLLED DICES:</b>";
    protected const string OnReachedEnd = "<b>- ON REACHED END TILE:</b>";
    protected const string OnReached = "<b>- ON REACHED:</b>";
    protected const string OnAddedDamage = "<b>- ON ADDED DAMAGE TO THIS TILE:</b>";
    #endregion

    #region DAMAGE MODIFIERS 
    //Separated the logic of modifying in case we want to Override the logic and not the visuals
    //DO NOT CALL THESE FROM THE TILE LOGIC, THIS IS FOR OVERRIDING ONLY (Look at Tile_Creating for a good examples)
    //IF YOU WANT TO MULTIPLY DAMAGE CALL IT FROM Tile.MultiplyBaseDamage()
    public virtual float AddBaseDamage(float addedDamage)
    {
        Tile.SetBaseDamage(BaseDamage + addedDamage);
        return addedDamage;
    }
    public virtual float RemoveBaseDamage(float removedDamage)
    {
        if (removedDamage > BaseDamage)
        {
            removedDamage = BaseDamage;
        }
        Tile.SetBaseDamage(BaseDamage - removedDamage);
        return removedDamage;
    }
    public virtual void MultiplyBaseDamage(float mult)
    {
        Tile.SetBaseDamage(BaseDamage * mult);
    }
    #endregion
}
