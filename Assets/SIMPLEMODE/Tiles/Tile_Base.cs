using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public enum Intensity
{
    empty, low, mid, large
}
public class Tile_Base : MonoBehaviour
{
    [SerializeField] SecundarySkillsEnum secundarySkill;
    public int indexInBoard;

    [Header("Color testing")]
    public Color tileColor;
     SpriteRenderer tileTestSprite;
    TextMeshPro TMP_IndexDisplay;

    protected GameController_Simple GameController;
    protected Board_Controller_simple BoardController;
    private void Awake()
    {
        GameController = GameController_Simple.Instance;
        BoardController = Board_Controller_simple.Instance;

        tileTestSprite = GetComponentInChildren<SpriteRenderer>();
        TMP_IndexDisplay = GetComponentInChildren<TextMeshPro>();

        UpdateTileVisuals();
       
    }
    public void CopyVisualData(Tile_Base copyingTile)
    {
        tileColor = copyingTile.tileColor;
    }

    public void UpdateTileVisuals()
    {
        tileTestSprite = GetComponentInChildren<SpriteRenderer>();
        tileTestSprite.color = tileColor;
        TMP_IndexDisplay.text = indexInBoard.ToString();
    }
    public void FirstAppeareanceAnim()
    {
        float duration = 1;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
    }
    public IEnumerator OnPlacedInBoard() { yield break; } //animation only, the logic should be handled by the board_controller
    public IEnumerator OnReplacedInBoard() { yield break; }
    void shakeTile(Intensity intensity)
    {
        switch (intensity)
        {
            case Intensity.empty: break;
            case Intensity.low:
                transform.DOShakeRotation(0.2f, .1f, 1);
                break;
            case Intensity.mid:
                transform.DOShakeRotation(0.4f, .3f, 1);
                break;
            case Intensity.large:
                transform.DOShakeRotation(0.6f, .4f, 1);
                break;
        }
    }
    public virtual IEnumerator OnPlayerStepped()
    {
        switch (secundarySkill)
        {
            case SecundarySkillsEnum.extraDamage:
                shakeTile(Intensity.low);
                yield return GameController_Simple.Instance.AddAcumulatedDamage(20);
                break;
            case SecundarySkillsEnum.extraMoney:
                shakeTile(Intensity.low);
                 GameController_Simple.Instance.AddMoney(5);
                break;
            case SecundarySkillsEnum.empty:
                break;
        }
        yield break;

        //INHERITEDS
        //gamelogic
        //visual feedback
        //further logic

    }
    public virtual IEnumerator OnPlayerLanded()
    {
        shakeTile(Intensity.mid);
        yield break;
    }
    public virtual float GetLandedDamageAmount()
    {
        return 0;
    }
}
