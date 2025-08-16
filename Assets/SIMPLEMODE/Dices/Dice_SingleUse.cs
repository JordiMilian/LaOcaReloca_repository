using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Dice_SingleUse : Dice
{
    private void OnEnable()
    {
        Dices_Controller.Instance.OnDicesRolled.AddListener(OnDicesRolled);
    }
    private void OnDisable()
    {
        Dices_Controller.Instance.OnDicesRolled.RemoveListener(OnDicesRolled);
    }
    void OnDicesRolled(int i)
    {
        if(isSelectedForRoll && !isInShop)
        {
            Dices_Controller.Instance.availableDices.Remove(this);
            StartCoroutine(DestroyItself());
        }

        IEnumerator DestroyItself()
        {
            yield return new WaitForSeconds(Random.Range(0, 0.3f));
            transform.DOScale(Vector3.zero, 1).SetEase(Ease.InQuad) ;
            yield return new WaitForSeconds(1);

            Destroy(gameObject);
        }
    }

}
