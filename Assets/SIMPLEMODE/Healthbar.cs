using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] Image healthbarImage;
    [SerializeField] TextMeshProUGUI TMP_HP;
    public void UpdateHealthbar(float value, float maxValue)
    {

        DOTween.To(
            () => healthbarImage.fillAmount,
            x => { healthbarImage.fillAmount = x; },
            value / maxValue,
            .25f
            ).SetEase(Ease.OutBounce);
        string hp = MathJ.FloatToString(value, 1);
        string maxHp = MathJ.FloatToString(maxValue, 1);
        TMP_HP.text = $"{hp}/{maxHp}";
    }
}
