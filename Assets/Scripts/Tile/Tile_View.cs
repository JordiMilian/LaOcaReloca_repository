using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public abstract class Tile_View : MonoBehaviour
{
    //This script should have a lot of generic methods and coroutines, and each inherited child can use those methods as it pleases
    public Transform positionForPlayer;
    public abstract IEnumerator OnPlayerStepped();
    public abstract IEnumerator OnPlayerLanded();

    public void TileAppear()
    {
        float duration = 1;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
    }

    public enum textDisplay_type
    {
        AddMoney, AddDamage, MultiplyDamage, RegularText
    }
    public enum textDisplay_intensity
    {
        Small, Medium, Big
    }
    public IEnumerator DisplayText(textDisplay_type type, textDisplay_intensity intensity, string text)
    {

    }
}
