using DG.Tweening;
using UnityEngine;
using TMPro;

public struct transformStats
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}
public class TileSharedVisuals : MonoBehaviour
{
    Camera mainCamera;
    [HideInInspector] public transformStats originTransform;
    public bool canBeMoved = true;
    [SerializeField] float heightWhileDragged = .5f;
    GameController_Simple gameController;
    [HideInInspector] public Tile_Base tileBase;
    [SerializeField] TextMeshPro TMP_DamageDisplay;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        gameController = GameController_Simple.Instance;
        tileBase = GetComponent<Tile_Base>();
    }
    private void Start()
    {
        HideTooltip();
    }
    #region Set Origin
    public void SetOriginTransformWithTransform(Transform originTf)
    {
        originTransform.position = originTf.position;
        originTransform.rotation = originTf.rotation;
        originTransform.scale = originTf.localScale;
    }
    public void SetOriginTransformWithStats(transformStats stats)
    {
        originTransform.position = stats.position;
        originTransform.rotation = stats.rotation;
        originTransform.scale = stats.scale;
    }
    #endregion
    public void MoveTileToOrigin()
    {
        transform.DOMove(originTransform.position, .3f).SetEase(Ease.OutBounce);
        transform.rotation = originTransform.rotation;
    }
    public void PlaceTileInOrigin()
    {
        transform.position = originTransform.position;
        transform.rotation = originTransform.rotation;
    }
    public void UpdateDmgDisplayText()
    {
        TMP_DamageDisplay.text = MathJ.FloatToString(tileBase.GetDefaultCrossedDamage(), 1);
    }
    #region MOUSE INPUTS
    private void OnMouseDown()
    {
        if (!canBeMoved) { return; }
        if (tileBase == null) { tileBase = GetComponent<Tile_Base>();}
        if (tileBase.isBehindPlayer) { return; }
       

        if(TryGetComponent(out Tile_Base tile))
        {
            gameController.SelectedNewTile(tile);
        }
        HideTooltip();
    }
    private void OnMouseDrag()
    {
        if (!canBeMoved) { return; }
        if (tileBase.isBehindPlayer) { return; }
        if (GameController_Simple.Instance.currentGameState == GameState.MovingPlayer) { MoveTileToOrigin(); return; }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, Vector3.up * heightWhileDragged);


        if (plane.Raycast(ray, out float distance))
        {
            Vector3 mousePosInPlane = ray.GetPoint(distance);
            Debug.DrawLine(transform.position, mousePosInPlane);
            transform.position = Vector3.MoveTowards(transform.position, mousePosInPlane, 1);
        }
    }
    private void OnMouseUp()
    {
        if (!canBeMoved) { return; }
        if (tileBase.isBehindPlayer) { return; }

        if (gameController.CanPlaceTile())
        {
            gameController.PlaceTile();
        }
        else
        {
            MoveTileToOrigin();
        }   
    }
    [Header("Tooltip")]
    [SerializeField] GameObject TooltipRootGO;
    [SerializeField] TextMeshProUGUI TMP_description;
    [SerializeField] TextMeshProUGUI TMP_title;
    private void OnMouseEnter() { ShowTooltip(); }
    private void OnMouseExit() { HideTooltip(); }

    void HideTooltip()
    {
        TooltipRootGO.SetActive(false);
    }
    void ShowTooltip()
    {
        TMP_description.text = tileBase.GetTooltipText();
        TMP_title.text = tileBase.TitleText;
        TooltipRootGO.SetActive(true);
    }
    #endregion
    [SerializeField] float verticalShakeForce = 0.02f;
    float shakeDuration = 1;
    float lastShakeTime = 0;
    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.TryGetComponent(out Dice dice))
        {
            if(collision.contacts[0].impulse.y > 4f && lastShakeTime + shakeDuration < Time.time)
            {
                lastShakeTime = Time.time;
               transform.DOShakePosition(
               1,
               Vector3.up * verticalShakeForce,
               5
               );
            }
           
        }
    }
    #region SHARED ANIMATIONS
    public void FirstAppeareanceAnim()
    {
        float duration = 1;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
    }
    public void shakeTile(Intensity intensity)
    {
        switch (intensity)
        {
            case Intensity.empty: break;
            case Intensity.low:
                transform.DOShakeRotation(0.2f, 5f, 4);
                break;
            case Intensity.mid:
                transform.DOShakeRotation(0.4f, 10f, 8);
                break;
            case Intensity.large:
                transform.DOShakeRotation(0.6f, 20f, 10);
                break;
        }
    }
    [Header("Message display")]
    [SerializeField] TextMeshProUGUI messageDisplay;
    public void DisplayMessage(string message, TileMessageType messageType)
    {
        Color msgColor = Color.white;
        switch (messageType)
        {
            case TileMessageType.Neutral:
                msgColor = Color.white;
                break;
            case TileMessageType.AddPermaDamage:
                msgColor = Color.cyan;
                break;
            case TileMessageType.AddMultiplier:
                msgColor = Color.red;
                break;
            case TileMessageType.AddDamage:
                msgColor = Color.blue;
                break;
        }
        messageDisplay.color = msgColor;
        messageDisplay.text = message;

        Sequence msgSeq = DOTween.Sequence();
        msgSeq.Append(messageDisplay.rectTransform.DOScale(1, 0.5f)).
            Append(messageDisplay.rectTransform.DOShakeRotation(.2f, 10)).
            Append(messageDisplay.rectTransform.DOScale(0, 0.2f));

    }
    #endregion


}
