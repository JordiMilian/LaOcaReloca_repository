using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using System.Collections;
public class Dice : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IBuyable
{
    [Serializable]
    public struct DiceFaces
    {
        public int faceValue;
        public Transform faceTransform;
    }
    [SerializeField] DiceFaces[] diceFaces;
    public bool isSelectedForRoll;
    public int faceUpValue;
    public Rigidbody rb;
    public bool isMoving;
    protected bool isInShop = false;
    [SerializeField] int PriceInShop = 5;

    Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }
    public virtual int GetRollDiceValue()
    {
        return diceFaces[UnityEngine.Random.Range(0, diceFaces.Length)].faceValue;
    }
    private void Update()
    {
        isMoving = !rb.IsSleeping();

        if (!isMoving)
        {
            float highestHeight = Mathf.NegativeInfinity;
            int highestIndex = -1;
            for (int i = 0; i < diceFaces.Length; i++)
            {
                Transform faceTf = diceFaces[i].faceTransform;
                if (faceTf.position.y > highestHeight)
                {
                    highestHeight = faceTf.position.y;
                    highestIndex = i;

                }
            }
            faceUpValue = diceFaces[highestIndex].faceValue;
        }

    }

    [Header("Dragging")]
    [SerializeField] LayerMask layerMask;
    public bool canBeDragged = true;
    [SerializeField] float heightWhileDragged;
    Coroutine dragging;
    
    void AttemptStartDragging()
    {
        if(!canBeDragged) { return; }
        if(isInShop && GameController_Simple.Instance.CanPurchase(GetBuyingPrice()))
        {
            OnDiceBought();
        }
        dragging = StartCoroutine(draggingCoroutine());
        IEnumerator draggingCoroutine()
        {
            while(true)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                Plane plane = new Plane(Vector3.up, Vector3.up * heightWhileDragged);


                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 mousePosInPlane = ray.GetPoint(distance);
                    Debug.DrawLine(transform.position, mousePosInPlane);
                    transform.position = Vector3.MoveTowards(transform.position, mousePosInPlane, 1);
                }
                yield return null;
            }
        }
    }
    void StopDragging()
    {
        if(dragging != null)
        {
            StopCoroutine(dragging);
            dragging = null;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("clicked dice");
        AttemptStartDragging();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopDragging();
    }
    #region BUYING DICES
    public int GetBuyingPrice()
    {
        return PriceInShop;
    }
    
    public void OnAppearInShop(ShopItem_Controller shopItemController)
    {
       transform.position = shopItemController.buyablePositionTf.position;
       isInShop = true;
    }

    public void OnEnablePurchase()
    {
        canBeDragged = true;
    }

    public void OnDisablePurchase()
    {
        canBeDragged = false;
    }
    void OnDiceBought()
    {
        GameController_Simple gameController = GameController_Simple.Instance;
        gameController.RemoveMoney(GetBuyingPrice());
        gameController.shopController.GetShopItem(this).RemoveItem();
        Dices_Controller.Instance.availableDices.Add(this);
        isInShop = false;
        isSelectedForRoll = true;
    }
    #endregion
}

