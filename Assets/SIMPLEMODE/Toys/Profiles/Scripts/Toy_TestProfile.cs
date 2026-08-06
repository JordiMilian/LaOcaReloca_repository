using UnityEngine;

[CreateAssetMenu(menuName = "ToyProfile/TestProfile", fileName = "Toy_TestProfile")]
public class Toy_TestProfile : Toy_Info
{
    public override string GetTooltipDescription()
    {
        return "test profile";
    }

    public override void OnActivatedToy()
    {
        Debug.Log("Test Toy Activated");
    }

    public override void OnDeactivatedToy()
    {
        Debug.Log("Test Toy Deactivated");
    }
}
