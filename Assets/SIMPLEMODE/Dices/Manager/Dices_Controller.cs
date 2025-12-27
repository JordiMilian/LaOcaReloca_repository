
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class Dices_Controller : MonoBehaviour
{
    public List<Dice> availableDices = new List<Dice>();
    public static Dices_Controller Instance;

    //Maybe at some point we should make a LastDiceRoll_Info and have everything there
    public int LastRolledValue;
    public int LastRolledDicesCount;
    [SerializeField] Transform diceSpawnPoint;
    [SerializeField] float SpawnPos_Radius = 1;
    GameController_Simple gameController;

    private void Awake()
    {
        availableDices = GetComponentsInChildren<Dice>().ToList();
        Instance = this;
    }
    private void Start()
    {
        gameController = GameController_Simple.Instance;
    }
    public Button Button_Rolldices;
    [SerializeField] Button Button_AddExtraValue;
    [SerializeField] TextMeshProUGUI TMP_AddExtraValue, TMP_RollDicesText;
    public void EnableRollButton()
    {
        Button_Rolldices.interactable = true;
    }
    public void EnableAddExtraRollValueButton() { Button_AddExtraValue.interactable = true; }
    public void DisableAddExtraRollValueButton() {  Button_AddExtraValue.interactable = false;}
    public void DisableRollButton()
    {
        Button_Rolldices.interactable = false;
    }
    public IEnumerator RollDicesCoroutine()
    {
        List<Dice> dicesToRoll = GetDicesToRoll();
        foreach (Dice dice in dicesToRoll)
        {
            dice.RollDice();
        }

        //wait for at least 1 seconds so all dices can set up
        yield return new WaitForSeconds(1.2f);


        //Wait for all dices to stop moving and update the ones that do
        bool areAllDicesStopped;
        do
        {
            areAllDicesStopped = true;
            foreach (Dice dice in dicesToRoll)
            {
                if (!dice.rb.IsSleeping())
                {
                    areAllDicesStopped = false;
                    dice.UpdateFaceupValue();
                }
            }
            yield return null;
        }
        while (!areAllDicesStopped);


        int addedValue = 0;
        //first we add up all the values and then wait for the effects to aboid effects moving the dices
        foreach (Dice dice in dicesToRoll)
        {
            addedValue += dice.FaceUpValue;
        }
        foreach (Dice dice in dicesToRoll)
        {
            yield return dice.C_OnRolledEffect();
        }

        LastRolledValue = addedValue;
        LastRolledDicesCount = dicesToRoll.Count;

        TMP_AddExtraValue.rectTransform.DOShakeRotation(.1f, 10);
        yield return new WaitForSeconds(0.1f);
        LastRolledValue += boughtRollValue;
        ResetBoughtValue();
        SetDicesDraggable(true);
        SetMainButtonText( LastRolledValue.ToString());

    }
    public void SetMainButtonText(string text)
    {
        TMP_RollDicesText.text = text;
    }
    #region BUY ROLL VALUE
    public int boughtRollValue = 0;
    [SerializeField] int buyRollValuePrice = 1;

    public void Button_BuyExtraRollValue()
    {
        if (!gameController.CanPurchaseWithoutLosing(buyRollValuePrice)) { return; }

        GameController_Simple.Instance.RemoveMoney(buyRollValuePrice);
        AddBoughtValue(1);
    }
    public void AddBoughtValue(int amount)
    {
        boughtRollValue += amount;
        TMP_AddExtraValue.text = "+" + boughtRollValue.ToString();
    }
    void ResetBoughtValue()
    {
        boughtRollValue = 0;
        TMP_AddExtraValue.text = "+0";

    }
    #endregion

    void SetDicesDraggable(bool draggability)
    {
        foreach(Dice dice in availableDices)
        {
            dice.canBeDragged = draggability;
        }
    }
    public List<Dice> GetDicesToRoll()
    {
        List<Dice> dicesToRoll = new();
        foreach (Dice dice in availableDices)
        {
            if (dice.isSelectedForRoll) { dicesToRoll.Add(dice); }
        }
        return dicesToRoll;

    }
    public void SpawnNewDice(GameObject DicePrefab)
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * SpawnPos_Radius + diceSpawnPoint.position;
        Quaternion randomRot = UnityEngine.Random.rotation;
        GameObject newDice = Instantiate(DicePrefab, randomPos, randomRot, transform);
        availableDices.Add(newDice.GetComponent<Dice>());
    }
}
