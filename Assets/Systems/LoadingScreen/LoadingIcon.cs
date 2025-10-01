using DG.Tweening;
using UnityEngine;

public class LoadingIcon : MonoBehaviour
{
    void Start()
    {
        gameObject.transform.DORotate(Vector3.forward * 360f, 2f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.OutBounce);
    }
}
