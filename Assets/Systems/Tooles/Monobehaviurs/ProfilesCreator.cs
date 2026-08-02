using UnityEngine;

public class ProfilesCreator : MonoBehaviour
{

    [HideInInspector] [SerializeField] string assetName;
    [HideInInspector] [SerializeField] string folderName = "";
    [HideInInspector] [SerializeField] string title = "TITLE";
    [HideInInspector] [SerializeField] Color color = Color.white;
    [HideInInspector] [SerializeField] float baseDamage;
    [HideInInspector] [SerializeField] Rarity rarity = Rarity.Common;
    [HideInInspector] [SerializeField] TileTags tiletag = TileTags.NoTag;
    public TilesFactory factory;

    [HideInInspector][SerializeField] string ToyAssetName;
    [HideInInspector][SerializeField] string ToyTitle = "TOY TITLE";
    public ToysManager toysManager;

}
