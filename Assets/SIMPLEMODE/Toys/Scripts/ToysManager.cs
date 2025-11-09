using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ToysManager : MonoBehaviour
{
    public static ToysManager Instance;
    [SerializeField] Toy_Profile testProfile, testProfile2;
    [SerializeField] GameObject ToyPrefab;
    public List<Toy_Profile> AllToyProfiles;

    List<Toy_Controller> instantiatedToys = new List<Toy_Controller>();
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    public Toy_Controller InstantiateToy(Toy_Profile profile)
    {
        GameObject newToyGO = Instantiate(ToyPrefab);
        Toy_Controller newController = newToyGO.GetComponent<Toy_Controller>();
        newController.SetProfile(profile);

        instantiatedToys.Add(newController);

        return newController;
    }

    public Toy_Profile GetRandomToyProfile()
    {
        int randomIndex = Random.Range(0, AllToyProfiles.Count);
        return AllToyProfiles[randomIndex];
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

    #region TESTING
    [ContextMenu("Test Instantiate Toy")]
    void TestInstantiateToy()
    {
        Toy_Controller testToy =  InstantiateToy(testProfile);
        testToy.transform.position = new Vector3(0, 3, 0);

    }

    [ContextMenu("Test Instantiate Toy2")]
    void TestInstantiateToy2()
    {
        Toy_Controller testToy = InstantiateToy(testProfile2);
        testToy.transform.position = new Vector3(0, 3, 0);

    }
    #endregion

}
