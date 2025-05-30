using DG.Tweening;
using UnityEngine;

public class TileMovement : MonoBehaviour
{
    Camera mainCamera;
    Vector3 originPosition;
    public bool canBeDragged = true;
    [SerializeField] float heightWhileDragged = .5f;
    [SerializeField] float secondsToReturnAfterDrag = .3f;
    GameController_Simple gameController;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        gameController = GameController_Simple.Instance;
    }
    #region VISUALS
    private void OnMouseDown()
    {
        originPosition = transform.position;

        if(TryGetComponent(out Tile_Base tile))
        {
            gameController.SelectedNewTile(tile);
        }
        

    }
    private void OnMouseDrag()
    {
        if (!canBeDragged) { return; }

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
        if(gameController.CanPlaceTile())
        {
            gameController.PlaceTile();
        }
        else
        {
            MoveTileTo(originPosition);
        }   
    }
    public void MoveTileTo(Vector3 newPos)
    {
        transform.DOMove(newPos, .3f).SetEase(Ease.OutBounce);
    }
    #endregion
}
