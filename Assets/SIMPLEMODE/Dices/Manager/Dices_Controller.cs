
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
    public UnityEvent<int> OnDicesRolled;
    public int LastRolledValue;
    [SerializeField] float multiplyDicesRotationForce = 2, verticalDiceForce = 1;
    [SerializeField] Transform diceSpawnPoint;
    [SerializeField] float RollDicePos_Radius = 2, SpawnPos_Radius = 1;
    GameController_Simple gameController;

    [SerializeField] TextMeshProUGUI TMP_ButtonText;
    public Button Button_RollDice;
    [Header("Money to Roll")]
    public int MoneyToRoll = 1;
    private void Awake()
    {
        availableDices = GetComponentsInChildren<Dice>().ToList();
        Instance = this;
    }
    private void Start()
    {
        gameController = GameController_Simple.Instance;
    }
    public void EnableRollButton() 
    { 
        Button_RollDice.interactable = true;
        Button_BuyExtraValue.interactable = true;
    }
    public void DisableRollButton()
    {
        Button_RollDice.interactable = false;
        Button_BuyExtraValue.interactable = false;
    }

    //After using this coroutine, remember to call EnableRollButton() to re-enable the button
    public IEnumerator RollDicesCoroutine()
    {
        DisableRollButton();
        SetDicesDraggable(false);
        //Group up the dices into transform position
        float groupUpTime = 0.4f;
        List<Dice> dicesToRoll = GetDicesToRoll();
        foreach (Dice dice in dicesToRoll)
        {
            Transform diceTf = dice.transform;
            dice.rb.isKinematic = true;
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * RollDicePos_Radius + transform.position;
            diceTf.DOMove(randomPos, groupUpTime);
            diceTf.DORotate(UnityEngine.Random.rotation.eulerAngles, groupUpTime).SetEase(Ease.OutCubic);
        }
        yield return new WaitForSeconds(groupUpTime);

        //Add force to them
        foreach (Dice dice in dicesToRoll)
        {
            dice.rb.isKinematic = false;
            dice.rb.AddTorque(UnityEngine.Random.insideUnitSphere * multiplyDicesRotationForce, ForceMode.Impulse);
            dice.rb.AddForce(-Vector3.up * verticalDiceForce);
        }

        yield return new WaitForSeconds(0.1f);

        //Wait for all dices to stop moving
        bool areAllDicesStopped;
        do
        {
            areAllDicesStopped = true;
            foreach (Dice dice in dicesToRoll)
            {
                if (dice.isMoving)
                {
                    areAllDicesStopped = false;
                    break;
                }
            }
            yield return null;
        }
        while (!areAllDicesStopped);


        int addedValue = 0;
        foreach (Dice dice in dicesToRoll)
        {
            addedValue += dice.faceUpValue;
        }
        LastRolledValue = addedValue;

        TMP_ButtonText.text = LastRolledValue.ToString();
        TMP_boughtRollValue.rectTransform.DOShakeRotation(.1f, 10);
        yield return new WaitForSeconds(0.1f);
        SetDicesDraggable(true);
        LastRolledValue += boughtRollValue;
        ResetBoughtValue();

        OnDicesRolled.Invoke(LastRolledValue);

        //
        void ResetBoughtValue()
        {
            boughtRollValue = 0;
            TMP_boughtRollValue.text = "+0";

        }
    }
    #region BUY ROLL VALUE
    int boughtRollValue = 0;
    [SerializeField] TextMeshProUGUI TMP_boughtRollValue;
    [SerializeField] Button Button_BuyExtraValue;
    public void Button_BuyExtraRollValue()
    {
        if (gameController.GetCurrentMoney() == 0) { return; }

        boughtRollValue++;
        GameController_Simple.Instance.RemoveMoney(1);
        TMP_boughtRollValue.text = "+" + boughtRollValue.ToString();
    }
    
    #endregion

    public void SetDicesDraggable(bool draggability)
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
