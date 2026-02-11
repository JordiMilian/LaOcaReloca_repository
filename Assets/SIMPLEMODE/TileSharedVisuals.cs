using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class TileSharedVisuals : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] float heightWhileDragged = .5f;
    GameController_Simple gameController;
    [HideInInspector] public TileController tileBase;
    [SerializeField] TextMeshProUGUI TMP_DamageDisplay;
    float messageDurationMultiplier = 1; 
    
    private void Awake()
    {
        mainCamera = Camera.main;
        gameController = GameController_Simple.Instance;
        tileBase = GetComponent<TileController>();
        SetBasicPanelColor();
    }
    public void UpdateDmgDisplayText()
    {
        TMP_DamageDisplay.text = MathJ.FloatToString(tileBase.GetBaseDamage(), 1);
    }
    public void SetBasicPanelColor()
    {
        tileBase.tileMaterial.SetFloat("_disabledAmount", 0f);
    }
    public void SetBasicPanelColor_Transparent()
    {
        tileBase.tileMaterial.SetFloat("_disabledAmount", 0.35f);
    }
    #region SHARED ANIMATIONS
    public void FirstAppeareanceAnim()
    {
        float duration = 1;
        transform.localScale = Vector3.zero;
        transform.DOScale(1, duration).SetEase(Ease.OutBounce);
    }
    Coroutine shakeCoroutine;
    public void shakeTile(Intensity intensity)
    {
        if (shakeCoroutine != null) { StopCoroutine(shakeCoroutine); }
        float sTime = 0;
        switch (intensity)
        {
            case Intensity.empty: break;
            case Intensity.low:
                sTime = .2f;
                transform.DOShakeRotation(sTime, 5f, 4);
                break;
            case Intensity.mid:
                sTime = 0.4f;
               transform.DOShakeRotation(sTime, 10f, 8);
                break;
            case Intensity.large:
                sTime = .6f;
                transform.DOShakeRotation(sTime, 20f, 10);
                break;
        }
        shakeCoroutine = StartCoroutine( shakeDelay(sTime));

        IEnumerator shakeDelay(float time)
        {
            yield return new WaitForSeconds(time);
            transform.rotation = Quaternion.identity;
        }
    }
    [Header("Message display")]
    [SerializeField] TextMeshProUGUI messageDisplay;
    public IEnumerator DisplayMessage(string message, TileMessageType messageType)
    {
        Color msgColor = Color.white;
        switch (messageType)
        {
            case TileMessageType.Neutral:
                msgColor = Color.white;
                break;
            case TileMessageType.AddBaseDamage:
                msgColor = Color.cyan;
                break;
            case TileMessageType.AddMultiplier:
                msgColor = Color.red;
                break;
            case TileMessageType.DealDamage:
                msgColor = Color.blue;
                break;
        }
        messageDisplay.color = msgColor;
        messageDisplay.text = message;

        float durationMultiplier = MessajesManager.instance.GetDurationMultiplier();
        

        Sequence msgSeq = DOTween.Sequence();
        msgSeq.Append(messageDisplay.rectTransform.DOScale(1, 0.2f * durationMultiplier)).
            Append(messageDisplay.rectTransform.DOShakeRotation(.4f * durationMultiplier, 10)).
            Append(messageDisplay.rectTransform.DOScale(0, 0.2f * durationMultiplier));
        float waitTime = .6f * durationMultiplier;
        if(messageType == TileMessageType.DealDamage) { waitTime /= 2; }
        yield return new WaitForSeconds(waitTime);
    }
    #endregion


}
