using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class TileSharedVisuals : MonoBehaviour
{
    Camera mainCamera;
    [HideInInspector] public transformData originTransform;
    public bool canBeMoved = true;
    [SerializeField] float heightWhileDragged = .5f;
    GameController_Simple gameController;
    [HideInInspector] public TileController tileBase;
    [SerializeField] TextMeshProUGUI TMP_DamageDisplay;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        gameController = GameController_Simple.Instance;
        tileBase = GetComponent<TileController>();
        SetBasicPanelColor();
    }
    
    #region Set Origin
    //The rotation of a Tile is ALWAYS identity so its always facing the player
    public void SetOriginTransformWithTransform(Transform originTf)
    {
        originTransform.position = originTf.position;
        originTransform.rotation = Quaternion.identity;
        originTransform.scale = originTf.localScale;
    }
    public void SetOriginTransformWithStats(transformData stats)
    {
        originTransform.position = stats.position;
        originTransform.rotation = Quaternion.identity;
        originTransform.scale = stats.scale;
    }
    #endregion
    public void MoveTileToOrigin()
    {
        float duration = .3f;
        Sequence sequence = DOTween.Sequence();
        sequence.
            Append(transform.DOMove(originTransform.position, duration).SetEase(Ease.OutBack)).
            Join(transform.DOScale(originTransform.scale, duration).SetEase(Ease.OutCubic)).
            Join(transform.DORotateQuaternion(originTransform.rotation, duration).SetEase(Ease.OutBounce));
    }
    public void PlaceTileInOrigin()
    {
        transform.position = originTransform.position;
        transform.rotation = originTransform.rotation;
        transform.localScale = originTransform.scale;
    }
    public void UpdateDmgDisplayText()
    {
        TMP_DamageDisplay.text = MathJ.FloatToString(tileBase.GetBaseDamage(), 1);
    }
    public void SetBasicPanelColor()
    {
        //TO DO: Add something to the material to show dragability
        //basicColorPanel.color = tileBase.tileColor;
    }
    public void SetBasicPanelColor_Transparent()
    {
       // basicColorPanel.color = new Color(tileBase.tileColor.r, tileBase.tileColor.g, tileBase.tileColor.b, 0.75f);
    }
    #region TOOLTIPS
    private void OnMouseDown()
    {
        if (!canBeMoved) { return; }
        if (tileBase == null) { tileBase = GetComponent<TileController>();}
        if (tileBase.isBehindPlayer) { return; }
       

        if(TryGetComponent(out TileController tile))
        {
            gameController.SelectedNewTile(tile);
        }
   
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
        transform.DOScale(originTransform.scale, duration).SetEase(Ease.OutBounce);
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

        Sequence msgSeq = DOTween.Sequence();
        msgSeq.Append(messageDisplay.rectTransform.DOScale(1, 0.5f)).
            Append(messageDisplay.rectTransform.DOShakeRotation(.2f, 10)).
            Append(messageDisplay.rectTransform.DOScale(0, 0.2f));

    }
    #endregion


}
