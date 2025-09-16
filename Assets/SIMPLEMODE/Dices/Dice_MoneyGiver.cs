using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Dice_MoneyGiver : Dice_BasicDice
{
    int moneyToGet;
    public override void UpdateFaceupValue()
    {
        base.UpdateFaceupValue();
        moneyToGet = FaceUpValue;
        FaceUpValue = 0;
    }
    public override IEnumerator C_OnRolledEffect()
    {
        yield return base.C_OnRolledEffect();
        yield return new WaitForSeconds(0.2f);

        GameController_Simple.Instance.AddMoney(moneyToGet);

        StartCoroutine(DestroyItself());
        yield break;

        IEnumerator DestroyItself()
        {
            Dices_Controller.Instance.availableDices.Remove(this);
            yield return new WaitForSeconds(Random.Range(0, 0.3f));
            transform.DOScale(Vector3.zero, 1).SetEase(Ease.InQuad);
            yield return new WaitForSeconds(1);

            Destroy(gameObject);
        }
    
    }
    public override string GetTooltipDescription()
    {
        return $"Gives money instead of face up value. Is destroyed after rolled.";
    }
}
