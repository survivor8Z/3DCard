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

    //��Щ����ֱ��������д
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


    //��TableCardת�����
    [HideInInspector]public TableCardBase theTableCardBase; //�����ʵ�忨��,���ж�Ӧ��TableCardBase..

    [HideInInspector]public HandCardVisual theHandCardVisual;

    #region �������ں���
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
    /// ��HandCardDeck����
    /// </summary>
    public void Deleted(int index)
    {
        if (this.index != index) return;
        //Destroy(gameObject);//TODO:�����֮����˵
        PoolMgr.Instance.PushObj(gameObject);
    }

    public void TranslateToTableCard(int index)
    {
        if (this.index != index) return;
        //TODO:���˿������ó�TableCard
        

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

    #region ʵ�ʿ��ƴ��Ч�����
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

    //�¼���Ӧ


    #region ugui����ӿ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (isSelected) return;
        if (isDragging) return;

        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardHovered, index);
        isHovered = true;
        handCardDeck.hoveredCard = this; // ���õ�ǰ��ͣ������
        //��Ч
        MusicMgr.Instance.PlaySound("card#"+UnityEngine.Random.Range(7, 9).ToString(),
                                    transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (isDragging) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardHoveredExit, index);
        isHovered = false;
        if (handCardDeck.hoveredCard == this) // �����ǰ��ͣ�����������
        {
            handCardDeck.hoveredCard = null; // �����ͣ״̬
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
                handCardDeck.dragedCard = this; // ���õ�ǰ��ק������
                isDragging = true;
            }
        }
        else if(cardSO.cardType == E_CardType.E_Modificatory)
        {
            return;
        }
        else if(cardSO.cardType == E_CardType.E_Behavior)
        {
            
            handCardDeck.dragedCard = this; // ���õ�ǰ��ק������
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

    #region ���ƴ������
    //1
    //������ת��Ϊ����
    public void PlaceCard()
    {
        Debug.Log("PlaceCard");
        //����֪ͨ���ƿ���һ��index�����뿪��,���ƿ����֪ͨHandCardBase�뿪����,����TranslateToTableCard,������ת��Ϊ����
        handCardDeck.TheCardLeaveHandDeck(index);
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

        handCardDeck.table.tableCardsControl.tableRootCards.Add(theTableCardBase);
    }

    //2
    //��1������ת��Ϊ���ƻ�����,���õ�������
    public void PlaceToTableCard(TableCardBase theTableCard)
    {
        handCardDeck.TheCardLeaveHandDeck(index);
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

        //���ó�Ϊ������Ƶ��Ӷ���
        this.theTableCardBase.TryStackToTheTableCard(theTableCard);
    }
    #endregion
}
