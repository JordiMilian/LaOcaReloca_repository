using UnityEngine;
using System.Collections;
using static StringTools;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
public class TileStateClass : ScriptableObject
{
    public float BaseDamage = 10;
    public string Title = "NO TITLE";
    public Color tileColor = Color.gray;
    public Texture tileTexture;
    [HideInInspector] public Rarity rarity = Rarity.none;
    public TileSize tileSize = TileSize.Medium;
    [HideInInspector] public int uniquePrice = 0; //IF rarity is Unique, use this value.
    public TileTags[] tileTags;
    public List<GenericSkills> genericSkills = new();
    [HideInInspector] public TileController _Tile;
    protected TileSharedVisuals tileMovement;
    [Space(5)]
    protected Board_Controller_simple BoardController;
    protected GameController_Simple GameController;
    public int StepsToCross = 1;
    public void SetStepsToCross(int newSteps)
    {
        StepsToCross = newSteps; remainingSteps = newSteps;
        if(newSteps == 1) { tileSize = TileSize.Medium; }
        else if(newSteps > 1) { tileSize = TileSize.Big; }
        else {  tileSize = TileSize.Small; }
        BoardController.UpdateStructData();
        BoardController.MoveTiles_ToTfData(true);
    }
    [HideInInspector] public int remainingSteps = 1;
    
    public void Initialize()
    {
        BoardController = Board_Controller_simple.Instance;
        GameController = GameController_Simple.Instance;
        tileMovement = _Tile.tileMovement;
    }
    #region VIRTUAL LOGIC
    public virtual IEnumerator OnPlayerStepped()
    {
        yield return GameController.OnCrossed_CardEffects.C_ActivateEffects(_Tile);

        _Tile.DamagesToDeal.Add(_Tile.GetModifiedBaseDamage());
        _Tile.DamagesToDeal.Reverse();        
    }
    public virtual IEnumerator OnPlayerLanded()
    {
        if (genericSkills.Contains(GenericSkills.ExtraDiceroll)) { GameController.SetRemainingRolls(GameController.RollsRemaining +1); }
        yield break;

    }
    public virtual IEnumerator OnTileFinished() //Triggered just before stepping out of a tile or after landing. 
    {
        yield return _Tile.C_DealAllDamageToDeal();

        if(genericSkills.Contains(GenericSkills.Golden))
        {
            GameController.AddMoney(1);
        }
        if (genericSkills.Contains(GenericSkills.Fragile))
        {
            yield return BoardController.C_RemoveTile(_Tile.indexInBoard);
        }
    }
    public virtual void OnSteppedOut()
    {
        remainingSteps = StepsToCross;
    }
    public virtual IEnumerator OnPlacedInBoard() { remainingSteps = StepsToCross ; yield break; }
    public virtual IEnumerator OnRemovedFromBoard() { yield break; }
    #endregion
    #region TOOLTIP TEXT
    public virtual string GetGenericSkillsText()
    {
        List<string> skillStrings = new();

        string stepsString = "";
        if (StepsToCross == 0) { stepsString = "0 steps"; }
        else if (StepsToCross > 1) { stepsString = $"{StepsToCross} steps"; }
        if(stepsString.Length > 0) { skillStrings.Add(CustomSkill(stepsString)); }
        
        foreach (GenericSkills skill in genericSkills) { skillStrings.Add(CustomSkill(skill.ToString())); }

        if (tileTags.Contains(TileTags.Token)) { skillStrings.Add(CustomSkill("Token")); }

        string finalString = "";
        for (int i = 0; i < skillStrings.Count; i++)
        {
            finalString += skillStrings[i];
            if (i < skillStrings.Count - 1) { finalString += ", "; }
        }
        if (skillStrings.Count > 0) { finalString += "\n"; }

        return finalString;
    }
    public virtual string GetTooltipText() 
    {
        return "";
    }
    #region TOOLTIP INTRO
    protected const string OnCrossed = "<b>- ON CROSSED:</b>";
    protected const string OnLanded = "<b>- ON LANDED:</b>";
    protected const string OnRolledDice = "<b>- ON ROLLED DICES:</b>";
    protected const string OnReachedEnd = "<b>- ON REACHED END TILE:</b>";
    protected const string OnReached = "<b>- ON REACHED:</b>";
    protected const string OnAddedDamage = "<b>- ON ADDED DAMAGE TO THIS TILE:</b>";
    protected const string OnAddedNewTileToBoard = "<b>- ON ADDED NEW TILE TO BOARD:</b>";
    protected const string OnEnterInBoard = "<b>- ON ENTER BOARD:</b>";
    protected string OnLandedOnTag(TileTags tag) { return $"<b>- ON LANDED ON AN {tag.ToString().ToUpper()} TILE:</b>"; }
    protected string OnCrossedOnTag(TileTags tag) { return $"<b>- ON CROSSED A {tag.ToString().ToUpper()} TILE:</b>"; }
    #endregion
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
            List<ProfilesGroup> properGroups = registry.GetProfileGroups(this);
            foreach (ProfilesGroup group in properGroups)
            {
                group.tilesList.Add(this);
                EditorUtility.SetDirty(group);
            }
        } 
    }
#endif
}
