using UnityEngine;

public class CardsCreator : MonoBehaviour
{
    [HideInInspector] [SerializeField] string AssetName;
    [HideInInspector] [SerializeField] string DisplayName;
    [HideInInspector][SerializeField] float DefaultCrossedDamage;
    [HideInInspector][SerializeField] GameObject BaseCardPrefab;
    [HideInInspector][SerializeField] GameObject TargetPrefab;
    [HideInInspector][SerializeField] Color CardColor = Color.white;
    [HideInInspector][SerializeField] ShopController shopController;
    [HideInInspector][SerializeField] Rarity cardRarity = Rarity.Common;
    [HideInInspector][SerializeField] TileTags[] tileTags;

}
