using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class Player_View : MonoBehaviour
{
    public IEnumerator StepPlayerTo(Vector3 newPos)
    {
        float duration = 0.25f;
        Sequence seq =
            DOTween.Sequence().
                Append(transform.DOMove(newPos, duration)).SetEase(Ease.InCubic)
                ;
        //cancelar y superposiciones
        yield return new WaitForSeconds(duration);
    }
    public void LandPlayerHere()
    {
        transform.DOShakePosition(0.2f, .1f, 1);
    }
    
}
