using System.Collections.Generic;
using UnityEngine;

public class TileConfig : ScriptableObject
{
    public float BaseDamage = 10;
    public string Title = "NO TITLE";
    public Color tileColor = Color.gray;
    public Texture tileTexture;
    public Rarity rarity = Rarity.none;
    public TileSize tileSize = TileSize.Medium;
    [HideInInspector] public int uniquePrice = 0; //IF rarity is Unique, use this value.
    public TileTags[] tileTags;
    public List<GenericSkills> genericSkills = new();
    public int StepsToCross = 1;

    public object UniqueData;
}
