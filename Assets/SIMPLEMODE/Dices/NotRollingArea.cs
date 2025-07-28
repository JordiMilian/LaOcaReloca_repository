using UnityEngine;

public class NotRollingArea : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Dice dice))
        {
            dice.isSelectedForRoll = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Dice dice))
        {
            dice.isSelectedForRoll = true;
        }
    }
}
