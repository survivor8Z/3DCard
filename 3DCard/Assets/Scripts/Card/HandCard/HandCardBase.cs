using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

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
    

    public HandCardDeck handCardDeck;
    public RectTransform slotRectTrans;
    public Transform selectedToPos=>handCardDeck.handCardDeckVisual.handCardSelectedToPos;
    public int index;
    public bool isHovered = false;
    public bool isSelected = false; 
    public bool isDragging = false;


    //与TableCard转换相关
    [HideInInspector]public TableCardBase theTableCardBase; //如果是实体卡牌,则有对应的TableCardBase..

    [HideInInspector]public HandCardVisual theHandCardVisual;

    #region 生命周期函数
    private void Awake()
    {

        theHandCardVisual = GetComponent<HandCardVisual>();
        theTableCardBase = GetComponent<TableCardBase>();
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardDel, Deleted);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardLeaveHandCardDeck, TranslateToTableCard);
    }


    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardDel, Deleted);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardLeaveHandCardDeck, TranslateToTableCard);
    }
    #endregion



    /// <summary>
    /// 由HandCardDeck调用
    /// </summary>
    public void Deleted(int index)
    {
        if (this.index != index) return;
        //Destroy(gameObject);//TODO:对象池之后再说
        PoolMgr.Instance.PushObj(gameObject);
    }

    public void TranslateToTableCard(int index)
    {
        if (this.index != index) return;
        //TODO:将此卡牌设置成TableCard
        

        this.enabled = false;

        theTableCardBase.enabled = true;
    }


    public void ResetState()
    {
        isDragging = false;
        isSelected = false;
        ExecuteEvents.ExecuteHierarchy<IEndDragHandler>(
            gameObject,
            new PointerEventData(EventSystem.current),
            ExecuteEvents.endDragHandler
        );
    }

    #region 实际卡牌打出效果相关
    #region DragedPlay
    public virtual void TryCardDragPlay(IInteractable pointInteractableObject)
    {
        handCardDeck.dragedCard = null;
        isDragging = false;
    }
    public void FailDragPlay()
    {
        //UNDONE
        print("FailDragPlay");
    }
    #endregion


    #region SelectedPlay
    public virtual void TryCardSelectedPlay(IInteractable pointInteractableObject)
    {
        handCardDeck.selectedCard = null;
        isSelected = false;
    }
    public void FailSelectedPlay()
    {
        //UNDONE
        print("FailSelectedPlay");
    }
    #endregion
    #endregion

    //事件响应


    #region ugui输入接口
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (isSelected) return;
        if (isDragging) return;

        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardHovered, index);
        isHovered = true;
        handCardDeck.hoveredCard = this; // 设置当前悬停的手牌
        //音效
        MusicMgr.Instance.PlaySound("card#"+UnityEngine.Random.Range(7, 9).ToString(),
                                    transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (isDragging) return;
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
        if(cardSO.cardType == E_CardType.E_Entity)
        {
            if(handCardDeck.player.playerMove.inSceneObj is Table)
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_HandCardStartDrag, index);
                handCardDeck.dragedCard = this; // 设置当前拖拽的手牌
                isDragging = true;
            }
        }
        else if(cardSO.cardType == E_CardType.E_Modificatory)
        {
            return;
        }
        else if(cardSO.cardType == E_CardType.E_Behavior)
        {
            
            handCardDeck.dragedCard = this; // 设置当前拖拽的手牌
            isDragging = true;
        }
        else if(cardSO.cardType == E_CardType.E_Condition)
        {
            return;
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSelected) return;
        //EventCenter.Instance.EventTrigger(E_EventType.E_HandCardOnDrag, index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSelected) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardEndDrag, index);
        isDragging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardSO.cardType == E_CardType.E_Entity)
        {

        }
        else if (cardSO.cardType == E_CardType.E_Modificatory)
        {

        }
        else if (cardSO.cardType == E_CardType.E_Behavior)
        {
            return;
        }
        else if (cardSO.cardType == E_CardType.E_Condition)
        {
            
        }
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardPointDown, index);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging) return;
        if (!isHovered) return;
        if (cardSO.cardType == E_CardType.E_Entity)
        {

        }
        else if (cardSO.cardType == E_CardType.E_Modificatory)
        {

        }
        else if (cardSO.cardType == E_CardType.E_Behavior)
        {
            return;
        }
        else if (cardSO.cardType == E_CardType.E_Condition)
        {

        }
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardPointUp, index);
    }
    #endregion

    #region 卡牌打出方法
    //1
    //将手牌转换为桌牌
    public void PlaceCard()
    {
        Debug.Log("PlaceCard");
        //先是通知手牌库有一个index卡牌离开了,手牌库接着通知HandCardBase离开完了,触发TranslateToTableCard,将手牌转换为桌牌
        handCardDeck.TheCardLeaveHandDeck(index);
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

        handCardDeck.table.tableCardsControl.tableRootCards.Add(theTableCardBase);
    }

    //2
    //在1将手牌转换为桌牌基础上,放置到桌牌上
    public void PlaceToTableCard(TableCardBase theTableCard)
    {
        handCardDeck.TheCardLeaveHandDeck(index);
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

        //设置成为这个桌牌的子对象
        this.theTableCardBase.TryStackToTheTableCard(theTableCard);
    }
    #endregion
}
