using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Dice_BasicDice : Dice
{
    [SerializeField] float RollDicePos_Radius = 2;
    [SerializeField] float multiplierRotationForce = 2, verticalDiceForce = 1;


    public override void RollDice()
    {
        base.RollDice();
        StartCoroutine(rollDicesCoroutine());

        IEnumerator rollDicesCoroutine()
        {
            //Move to the center
            float moveToStartDuration = 0.5f;

            rb.isKinematic = true;
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * RollDicePos_Radius + Dices_Controller.Instance.transform.position;
            transform.DOMove(randomPos, moveToStartDuration);
            transform.DORotate(UnityEngine.Random.rotation.eulerAngles, moveToStartDuration).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(moveToStartDuration);

            //push
            rb.isKinematic = false;
            rb.AddTorque(UnityEngine.Random.insideUnitSphere * multiplierRotationForce, ForceMode.Impulse);
            rb.AddForce(-Vector3.up * verticalDiceForce);
        }
    }


    
}
