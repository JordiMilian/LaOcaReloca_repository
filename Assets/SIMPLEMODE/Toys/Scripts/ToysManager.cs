using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class ToysManager : MonoBehaviour
{
    public static ToysManager Instance;
    [SerializeField] ToyConfig testConfig, testConfig2;
    [SerializeField] GameObject ToyPrefab;
    public List<Toy_Info> AllToyProfiles;
    public ToySlot[] slots;
    public List<Toy_Controller> instantiatedToys = new List<Toy_Controller>();
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    public Toy_Controller InstantiateToyCopy(Toy_Info info)
    {
        GameObject newToyGO = Instantiate(ToyPrefab);
        Toy_Controller newController = newToyGO.GetComponent<Toy_Controller>();
        newController.SetProfile(info.GetCopy());

        instantiatedToys.Add(newController);

        return newController;

    }
    public Toy_Controller InstantiateToyFromConfig(ToyConfig config)
    {
        GameObject newToyGO = Instantiate(ToyPrefab);
        Toy_Controller newController = newToyGO.GetComponent<Toy_Controller>();
        config._Info.configId = config.name;
        newController.SetProfile(config._Info.GetCopy());
        //newController._Profile.configId = config.name;

        instantiatedToys.Add(newController);

        return newController;
    }

    public Toy_Info GetRandomToyProfile()
    {
        int randomIndex = Random.Range(0, AllToyProfiles.Count);
        return AllToyProfiles[randomIndex];
    }
    public void DestroyAllToysInSlots()
    {
        foreach(ToySlot slot in slots)
        {
            if (slot.isActive)
            {
                slot.currentToy.DeactivateToy();
                DestroyToy(slot.currentToy);
                slot.isActive = false;
            }
        }
    }
    public void DestroyToy(Toy_Controller toy)
    {
        instantiatedToys.Remove(toy);
        Destroy(toy.gameObject);
    }
    public void DisableToysDrag()
    {
        foreach (Toy_Controller toy in instantiatedToys)
        {
            toy.isDraggable = false;
        }
    }
    public void EnableToysDrag()
    {
        foreach (Toy_Controller toy in instantiatedToys)
        {
            toy.isDraggable = true;
        }
    }


}
