using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public abstract class Toy_Info
{
    public string Title;
    [HideInInspector] public string configId;
    public Mesh toyMesh; // no mole hauries de fer el mateix que amb la texture
    protected GameController_Simple _gameController;
    protected Board_Controller_simple _boardController;
    protected Toy_Controller _ToyController;

    public void InitializeProfile(Toy_Controller controller)
    {
        _gameController = GameController_Simple.Instance;
        _boardController = Board_Controller_simple.Instance;
        _ToyController = controller;
    }
    public abstract void OnActivatedToy();
    public abstract void OnDeactivatedToy();
    public abstract string GetTooltipDescription();

    public virtual Toy_Info GetCopy()
    {
        Toy_Info newInfo = (Toy_Info)Activator.CreateInstance(GetType());

        return CopyBaseStatsIntoOther(newInfo);
    }
    protected Toy_Info CopyBaseStatsIntoOther(Toy_Info otherProfile)
    {    
        otherProfile.toyMesh = toyMesh;
        otherProfile.Title = Title;
        otherProfile.configId = configId;
        return otherProfile;
    }
}
