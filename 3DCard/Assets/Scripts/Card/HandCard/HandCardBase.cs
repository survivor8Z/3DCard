using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

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
    public int cardID; // ����ID

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
    /// ��HandCardDeck����
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


    //�¼���Ӧ
    //ֱ�ӷŵ�HandCardDeck�д�����
    //public void OnClick(int index)
    //{
    //    if(index == this.index)
    //    {
    //        if (isSelected)
    //        {
    //            isSelected = false;
    //            handCardDeck.selectedCard = null; // ���ѡ��״̬
    //            return;
    //        }
    //        if (isDragging)
    //        {
    //            return;
    //        }

    //        isSelected = true;
    //        handCardDeck.selectedCard = this; // ���õ�ǰѡ�е�����
    //    }
    //    else
    //    {
    //        isSelected = false; 
    //        transform.SetParent(handCardDeck.transform);
    //    }
    //}



    // ugui����ӿ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardHovered, index);
        isHovered = true;
        handCardDeck.hoveredCard = this; // ���õ�ǰ��ͣ������
    }

    public void OnPointerExit(PointerEventData eventData)
    {
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
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardStartDrag, index);
        handCardDeck.dragedCard = this; // ���õ�ǰ��ק������
        
        
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
        //handCardDeck.dragedCard = null; // �����ק״̬
        isDragging = false;

        //Debug.Log("OnEndDrag: " + index);
        //���Ҫ�� HandCardDeck ,Player�д���
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
