using UnityEngine;

public class Dice_SingleUse : Dice
{
    private void OnEnable()
    {
        Dices_Controller.Instance.OnDicesRolled.AddListener(OnDicesRolled);
    }
    private void OnDisable()
    {
        Dices_Controller.Instance.OnDicesRolled.RemoveListener(OnDicesRolled);
    }
    void OnDicesRolled(int i)
    {
        if(isSelectedForRoll)
        {
            Dices_Controller.Instance.availableDices.Remove(this);
            Destroy(gameObject);
        }
        
    }
}
