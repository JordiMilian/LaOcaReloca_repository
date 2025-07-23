
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;

[DefaultExecutionOrder(-1)]
public class Dices_Controller : MonoBehaviour
{
    public List<Dice> availableDices = new List<Dice>();
    public static Dices_Controller Instance;
    public UnityEvent<int> OnDicesRolled;
    public int LastRolledValue;
    [SerializeField] float multiplyDicesRotationForce = 2, verticalDiceForce = 1;

    private void Awake()
    {
        GetChildDices();
        Instance = this;
    }
    public void GetChildDices()
    {
        availableDices = GetComponentsInChildren<Dice>().ToList();
    }
    public IEnumerator RollDicesCoroutine()
    {
        //Group up the dices into transform position
        float groupUpTime = 0.4f;
        foreach (Dice dice in availableDices)
        {
            if (dice.isSelectedForRoll)
            {
                Transform diceTf = dice.transform;
                dice.rb.isKinematic = true;
                diceTf.DOMove(transform.position, groupUpTime);
                diceTf.DORotate(UnityEngine.Random.rotation.eulerAngles, groupUpTime).SetEase(Ease.OutCubic) ;
            }
        }
        yield return new WaitForSeconds(groupUpTime);

        //Add force to them
        foreach (Dice dice in availableDices)
        {
            if (dice.isSelectedForRoll)
            {
                dice.rb.isKinematic = false;
                dice.rb.AddTorque(UnityEngine.Random.insideUnitSphere * multiplyDicesRotationForce, ForceMode.Impulse);
                dice.rb.AddForce(-Vector3.up * verticalDiceForce);
            }
        }

        yield return new WaitForSeconds(0.1f);

        //Wait for all dices to stop moving
        bool areAllDicesStopped;
        do
        {
            areAllDicesStopped = true;
            foreach (Dice dice in availableDices)
            {
                if (dice.isSelectedForRoll && dice.isMoving)
                {
                    areAllDicesStopped = false;
                    break;
                }
            }
            yield return null;
        }
        while (!areAllDicesStopped);


        int addedValue = 0;
        foreach (Dice dice in availableDices)
        {
            if (dice.isSelectedForRoll)
            {
                addedValue += dice.faceUpValue;
            }
        }
        LastRolledValue = addedValue;
    }
}
