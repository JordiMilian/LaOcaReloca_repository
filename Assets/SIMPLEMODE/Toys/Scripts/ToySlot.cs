using UnityEngine;

public class ToySlot : MonoBehaviour
{
    public bool isActive;
    public Toy_Controller currentToy;

    public void OnPlacedToyInSlot(Toy_Controller newToy)
    {
        if (newToy == currentToy)
        {
            currentToy.returnToyToOrigin();
        }

        if (newToy.isActive)
        {
            if(isActive)
            {
                moveActiveToys(newToy); //both toys are active, so switch place
            }
            else
            {
                replaceToy(newToy); //there is no slot here, so place it here
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
            newToy.returnToyToOrigin();

            newToy.ActivateToy();
        }

    }
    void SetNewSlot(Toy_Controller newToy)
    {
        currentToy = newToy;
        currentToy.currentSlot = this;
        currentToy.originTf = transform;
        currentToy.isActive = true;
        isActive = true;
    }
    void moveActiveToys(Toy_Controller newToy)
    {
        Toy_Controller oldToy = currentToy;
        ToySlot otherSlot = newToy.currentSlot;

        otherSlot.SetNewSlot(oldToy);
        SetNewSlot(newToy);

        oldToy.returnToyToOrigin();
        newToy.returnToyToOrigin();
    }
    void replaceToy(Toy_Controller newToy)
    {
        ToySlot otherSlot = newToy.currentSlot;

        otherSlot.currentToy = null;
        otherSlot.isActive = false;
        isActive = true;

        SetNewSlot(newToy);
        newToy.returnToyToOrigin();
    }
}
