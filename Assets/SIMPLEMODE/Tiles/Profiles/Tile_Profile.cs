using UnityEngine;
using System.Collections;
using static StringTools;
using UnityEditor;
using System.Collections.Generic;
public class Tile_Profile : ScriptableObject
{
    public float BaseDamage = 10;
    public string Title = "NO TITLE";
    public Color tileColor = Color.gray;
    public Texture tileTexture;
    [HideInInspector] public Rarity rarity = Rarity.none;
    public TileSize tileSize = TileSize.Medium;
    [HideInInspector] public int uniquePrice = 0; //IF rarity is Unique, use this value.
    public TileTags[] tileTags;
    [HideInInspector] public TileController _Tile;
    protected TileSharedVisuals tileMovement;
    [Space(5)]
    protected Board_Controller_simple BoardController;
    protected GameController_Simple GameController;
    
    public void Initialize()
    {
        BoardController = Board_Controller_simple.Instance;
        GameController = GameController_Simple.Instance;
        tileMovement = _Tile.tileMovement;
    }
    public virtual IEnumerator OnPlayerStepped()
    {
        _Tile.DamagesToDeal.Add(_Tile.GetModifiedBaseDamage());
        _Tile.DamagesToDeal.Reverse();

        yield return GameController.OnCrossed_CardEffects.C_ActivateEffects();
    }
    public virtual IEnumerator OnPlayerLanded() 
    {
        yield break;
    }
    public virtual IEnumerator OnPlacedInBoard() { yield break; }
    public virtual IEnumerator OnRemovedFromBoard() { yield break; }

    public virtual string GetTooltipText() { return "NO DESCRIPTION FOUND"; }
    #region TOOLTIP INTRO
    protected const string OnCrossed = "<b>- ON CROSSED:</b>";
    protected const string OnLanded = "<b>- ON LANDED:</b>";
    protected const string OnRolledDice = "<b>- ON ROLLED DICES:</b>";
    protected const string OnReachedEnd = "<b>- ON REACHED END TILE:</b>";
    protected const string OnReached = "<b>- ON REACHED:</b>";
    protected const string OnAddedDamage = "<b>- ON ADDED DAMAGE TO THIS TILE:</b>";
    protected const string OnAddedNewTileToBoard = "<b>- ON ADDED NEW TILE TO BOARD:</b>";
    protected const string OnEnterInBoard = "<b>- ON ENTER IN BOARD:</b>";
    protected string OnLandedOnTag(TileTags tag) { return $"<b>- ON LANDED ON AN {tag.ToString().ToUpper()} TILE:</b>"; }
    protected string OnCrossedOnTag(TileTags tag) { return $"<b>- ON CROSSED A {tag.ToString().ToUpper()} TILE:</b>"; }
    #endregion

    #region DAMAGE MODIFIERS 
    //Separated the logic of modifying in case we want to Override the logic and not the visuals
    //DO NOT CALL THESE FROM THE TILE LOGIC, THIS IS FOR OVERRIDING ONLY (Look at Tile_Creatine for a good examples)
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
#if UNITY_EDITOR
    //On validate, move this profile to the proper groups according to tags and rarity
    private void OnValidate()
    {
        if(EditorUtility.IsPersistent(this)) //check if the profile is in project window or an instance in memory. Only apply to project 
        {
            ProfileGroups_Registry registry = TilesGroupRegistry_singleton.Instance;

            //Remove from all groups
            foreach (ProfilesGroup group in registry.GetAllGroups())
            {
                if (group.tilesList.Remove(this))
                {
                    EditorUtility.SetDirty(group);
                }
            }

            //Add them to the proper groups
            List<ProfilesGroup> properGroups = registry.GetGroups(this);
            foreach (ProfilesGroup group in properGroups)
            {
                group.tilesList.Add(this);
                EditorUtility.SetDirty(group);
            }
        }
        
    }
#endif
}
