using UnityEngine;

public abstract class Toy_Profile : ScriptableObject
{
    public string Title;
    public Mesh toyMesh;
    protected GameController_Simple _gameController;
    protected Board_Controller_simple _boardController;
    protected Toy_Controller _ToyController;
    public Texture TooltipTexture;

    public void InitializeProfile(Toy_Controller controller)
    {
        _gameController = GameController_Simple.Instance;
        _boardController = Board_Controller_simple.Instance;
        _ToyController = controller;
    }
    public abstract void OnActivatedToy();
    public abstract void OnDeactivatedToy();
    public abstract string GetTooltipDescription();
}
