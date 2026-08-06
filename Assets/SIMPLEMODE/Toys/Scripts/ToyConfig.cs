using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ToyConfig", fileName = "NewConfig")]
public class ToyConfig : ScriptableObject
{
    [SerializeReference]
    [InlineProperty]
    public Toy_Info _Info;
    public Texture _texture;
}
