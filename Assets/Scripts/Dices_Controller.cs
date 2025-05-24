
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Dice
{
    public List<int> faces = new List<int>();
}

public class Dices_Controller : MonoBehaviour
{
    List<Dice> selectedDice = new List<Dice>();
    Dice defaulDice;
    List<int> defaulFaces = new List<int>() { 1, 2, 3, 4, 5, 6 };
    
    private void Awake()
    {
        defaulDice = new Dice();
        defaulDice.faces.AddRange(defaulFaces);
        selectedDice.Add(defaulDice);
    }
    public int RollDices()
    {
        int addedValue = 0;
        foreach(Dice dice in selectedDice)
        {
            addedValue += dice.faces[Random.Range(0, dice.faces.Count)];
        }
        return addedValue;
    }
}
