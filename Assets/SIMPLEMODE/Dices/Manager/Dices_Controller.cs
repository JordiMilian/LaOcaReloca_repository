
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class Dices_Controller : MonoBehaviour
{
    public List<Dice> availableDices = new List<Dice>();
    public static Dices_Controller Instance;
    public UnityEvent<int> OnDicesRolled;
    public int LastRolledValue { get; private set; }

    private void Awake()
    {
        GetChildDices();
        Instance = this;
    }
    public void GetChildDices()
    {
        availableDices = GetComponentsInChildren<Dice>().ToList();
    }
    public int RollDices()
    {
        int addedValue = 0;
        foreach(Dice dice in availableDices)
        {
            if(dice.isSelectedForRoll)
            {
                addedValue += dice.GetRollDiceValue();
            }
        }
        OnDicesRolled?.Invoke(addedValue);
        LastRolledValue = addedValue;
        return addedValue;
    }
}
