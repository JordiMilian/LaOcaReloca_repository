using UnityEngine;

public class ToySlot : MonoBehaviour
{
    public bool isActive;
    public Toy_Controller currentToy;

    public void OnPlacedToyInSlot(Toy_Controller newToy)
    {
        if (newToy == currentToy)
        {
            currentToy.returnToyToSlot();
        }

        if (newToy.isActive)
        {
            if(isActive)
            {
                replaceToys(newToy); //both toys are active, so switch place
            }
            else
            {
                changeToysSlot(newToy); //there is no slot here, so place it here
            }
        }
        else
        {
            if (isActive)
            {
                currentToy.DeactivateToy(); //there is already an active toy here, so deactivate it 
                ToysManager.Instance.DestroyToy(currentToy);
            }

            SetNewSlot(newToy);
            newToy.returnToyToSlot();

            newToy.ActivateToy();
        }

    }
    void SetNewSlot(Toy_Controller newToy)
    {
        currentToy = newToy;
        currentToy.currentSlot = this;
        currentToy.isActive = true;
        isActive = true;
    }
    void replaceToys(Toy_Controller newToy)
    {
        Toy_Controller oldToy = currentToy;
        ToySlot otherSlot = newToy.currentSlot;

        currentToy = newToy;
        otherSlot.currentToy = oldToy;

        oldToy.currentSlot = newToy.currentSlot;
        newToy.currentSlot = this;
        

        oldToy.returnToyToSlot();
        newToy.returnToyToSlot();
    }
    void changeToysSlot(Toy_Controller newToy)
    {
        ToySlot otherSlot = newToy.currentSlot;

        otherSlot.currentToy = null;
        otherSlot.isActive = false;
        isActive = true;

        SetNewSlot(newToy);
        newToy.returnToyToSlot();
    }
}
