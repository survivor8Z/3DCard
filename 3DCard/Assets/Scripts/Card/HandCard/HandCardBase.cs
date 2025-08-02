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
    , IDragHandler
    , IBeginDragHandler
    , IEndDragHandler
    , IPointerUpHandler
    , IPointerDownHandler
{
    public int cardID; // 卡牌ID

    public HandCardDeck handCardDeck;
    public RectTransform slotRectTrans;
    public Transform selectedToPos=>handCardDeck.handCardDeckVisual.handCardSelectedToPos;
    public int index;
    public bool isHovered = false;
    public bool isSelected = false; 
    public bool isDragging = false; 

    private void Awake()
    {
        
    }

    protected virtual void Start()
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
    
    
    public virtual bool CanDragPlayCard()
    {
        return true;
    }
    public virtual bool CanSelectedPlayCard()
    {
        return true;
    }
    public virtual void CardDragPlay()
    {
        if (!CanDragPlayCard())
        {

            return;
        }
        Debug.Log("CardDragPlay");


        handCardDeck.DelTheCard(index); 
    }
    public virtual void CardSelectedPlay()
    {
        if(!CanSelectedPlayCard())
        {
            return;
        }
        Debug.Log("CardSelectedPlay");
        handCardDeck.DelTheCard(index);
    }


    //事件响应
    //直接放到HandCardDeck中处理了
    //public void OnClick(int index)
    //{
    //    if(index == this.index)
    //    {
    //        if (isSelected)
    //        {
    //            isSelected = false;
    //            handCardDeck.selectedCard = null; // 清除选中状态
    //            return;
    //        }
    //        if (isDragging)
    //        {
    //            return;
    //        }

    //        isSelected = true;
    //        handCardDeck.selectedCard = this; // 设置当前选中的手牌
    //    }
    //    else
    //    {
    //        isSelected = false; 
    //        transform.SetParent(handCardDeck.transform);
    //    }
    //}



    // ugui输入接口
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
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSelected) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardStartDrag, index);
        handCardDeck.dragedCard = this; // 设置当前拖拽的手牌
        
        
        isDragging = true; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSelected) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardOnDrag, index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSelected) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardEndDrag, index);
        //handCardDeck.dragedCard = null; // 清除拖拽状态
        isDragging = false;

        //Debug.Log("OnEndDrag: " + index);
        //这个要到 HandCardDeck ,Player中处理
        //CardDragPlay();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardPointDown, index);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardPointUp, index);
        //Debug.Log("OnPointerUp: " + index);

    }
}
