using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class HandCardBase : CardBase
    , IPointerEnterHandler
    , IPointerExitHandler

    //这些可能直接在子类写
    //, IDragHandler
    //, IBeginDragHandler
    //, IEndDragHandler
    //, IPointerUpHandler
    //, IPointerDownHandler
{
    public HandCardDeck handCardDeck;
    public RectTransform slotRectTrans;
    public int index;
    public bool isHovered = false;

    private void Awake()
    {
        
    }

    private void Start()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardDel, Deleted);
    }


    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardDel, Deleted);
    }


    /// <summary>
    /// 由HandCardDeck调用
    /// </summary>
    public void Deleted(int index)
    {
        if (this.index != index) return;
        Destroy(gameObject);
    }











    public void OnPointerEnter(PointerEventData eventData)
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardHovered, index);
        isHovered = true;
        handCardDeck.hoveredCard = this; // 设置当前悬停的手牌
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardHoveredExit, index);
        isHovered = false;
        if (handCardDeck.hoveredCard == this) // 如果当前悬停的手牌是这个
        {
            handCardDeck.hoveredCard = null; // 清除悬停状态
        }
    }

    public virtual void CardPlay()
    {

    }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    Debug.Log("OnBeginDrag: " + index);
    //    EventCenter.Instance.EventTrigger(E_EventType.E_HandCardDraged, index);
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Debug.Log("OnDrag: " + index);
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    Debug.Log("OnEndDrag: " + index);
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerDown: " + index);
    //}
    //public void OnPointerUp(PointerEventData eventData)
    //{

    //}
}
