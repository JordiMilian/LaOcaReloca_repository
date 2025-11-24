using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TilesSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Board_Controller_simple board;
    bool isDragging = false;
    Camera mainCamera;
    [SerializeField] float minRange = 1;
    public TileController GetClosestTileInRange()
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;
        Vector3 thisPosition = transform.position;
        for (int i = 0; i < board.TilesList.Count; i++)
        {
            TileController otherTile = board.TilesList[i];
            float distance = Vector3.SqrMagnitude(thisPosition - otherTile.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        if(closestDistance > minRange * minRange)
        {
            return null;
        }
        return board.TilesList[closestIndex];
    }
    void Start()
    {
        board = Board_Controller_simple.Instance;
        mainCamera = Camera.main;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       isDragging = false;
    }
   
    void Update()
    {
        if (isDragging)
        {
            dragging();
        }

        //
        void dragging()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.zero);//drag at height 0 for now


            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mousePosInPlane = ray.GetPoint(distance);
                transform.position = mousePosInPlane;
            }
        }
    }
    
}
