using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;
using DG.Tweening;
public class Dice : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IBuyable, ITooltip, IPointerEnterHandler, IPointerExitHandler
{
    public int FaceUpValue;

    [Serializable]
    public struct DiceFaces
    {
        public int faceValue;
        public Transform faceTransform;
    }
    [SerializeField] protected DiceFaces[] diceFaces;

    public bool isSelectedForRoll;
    public Rigidbody rb;

    protected bool isInShop = false;
    [SerializeField] int PriceInShop = 5;

    Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }
    public virtual void RollDice() { canBeDragged = false; }//In this coroutine we should set the way this type of dice is rolled
    public virtual IEnumerator C_OnRolledEffect() { yield break; } //Effects happening after the dice has been rolled and stopped
    public virtual void UpdateFaceupValue()
    {
        FaceUpValue = diceFaces[GetHighestFaceIndex()].faceValue;
    } //this is virtual because some dices don't give faceUpValue (ex. MoneyDice)
    protected int GetHighestFaceIndex()
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
        return highestIndex;
    }

    #region DRAGGING
    [Header("Dragging")]
    public bool canBeDragged = true;
    [SerializeField] float heightWhileDragged;
    Coroutine dragging;
    void AttemptStartDragging()
    {
        if(!canBeDragged) { return; }
        if(isInShop && GameController_Simple.Instance.CanPurchaseWithoutLosing(GetBuyingPrice()))
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
        AttemptStartDragging();
        TooltipManager.Instance.ForceTooltip(this);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        StopDragging();
        TooltipManager.Instance.StopForcingThisTooltip(this);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.RequestTooltip(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.RemoveRequest(this);
    }
    #endregion
    #region TOOLTIP
    public virtual string GetTooltipDescription()
    {
        return $"Regular {diceFaces.Length} faces dice";
    }
    public string GetTooltipTitle()
    {
        return gameObject.name;
    }
    public Texture GetTooltipTexture()
    {
        return null;
    }
    #endregion
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

