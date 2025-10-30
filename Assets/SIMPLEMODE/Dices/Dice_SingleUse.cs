using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Dice_SingleUse : Dice_BasicDice
{
    [SerializeField] int uses = 1;
    [SerializeField] int usesRemaining = 1;
    public override IEnumerator C_OnRolledEffect()
    {
        usesRemaining--;
        if (usesRemaining <= 0)
        {
            StartCoroutine(DestroyItself());
        }

        
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
        return $"Destroyed after being rolled {uses}({usesRemaining}) times";
    }

}
