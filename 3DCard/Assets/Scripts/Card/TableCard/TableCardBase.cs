using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TableCardBase : CardBase
    , IPointerEnterHandler
    , IPointerExitHandler
    , IDragHandler
    , IBeginDragHandler
    , IEndDragHandler
    , IPointerUpHandler
    , IPointerDownHandler
{
    public Table table;
    public TableCardVisual theTableCardVisual;
    public HandCardBase thehandCardBase;
    public Player player;



    //����ק���
    public bool isDragging = false;
    //public UnityAction<TableCardBase> OnBeginDragEvent;
    public UnityAction<TableCardBase> OnDragEvent;
    public UnityAction<TableCardBase> OnEndDragEvent;

    //��ק���������
    public Slot toSlot;
    

    #region �������ں���
    private void Awake()
    {
        thehandCardBase = GetComponent<HandCardBase>();

        theTableCardVisual = GetComponent<TableCardVisual>();
    }
    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        //Debug.Log("TableCardBase Update");
    }
    private void OnDisable()
    {
        OnEndDragEvent -= table.tableCardsControl.OnTheTableCardEndDrag;
        OnDragEvent -= table.tableCardsControl.OnTheTableCardDrag;
    }
    #endregion
    //ֱ�ӵ�TableCardControlд��
    //public bool CanTableCardToHandCard()
    //{
    //    if(player.playerInteract.MouseViewPortPosition.y < 0.25f
    //       && cardSO.cardType == E_CardType.E_Entity
    //       && theHandCardBase.handCardDeck.cardPreList.Count < theHandCardBase.handCardDeck.maxCount
    //        /*&& isDragging//������¼�������*/)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    ////TODO:��Ҫ�������Ƶ����Ƶķ���,���ڱ�����д
    //public void TableCardToHandCard()
    //{
    //    if (!CanTableCardToHandCard()) return;

    //}

    protected void Init()
    {
        transform.SetParent(MapMgr.Instance.currentRoom.table.tableCardTransformParent);
        table = MapMgr.Instance.currentRoom.table;

        OnEndDragEvent += table.tableCardsControl.OnTheTableCardEndDrag;
        OnDragEvent += table.tableCardsControl.OnTheTableCardDrag;

        theTableCardVisual.enabled = true;
        theTableCardVisual.Init();
    }

    public void TranslateToHandCard(Slot currentToSlot)
    {
        enabled = false;
        theTableCardVisual.enabled = false;

        thehandCardBase.enabled = true;
        thehandCardBase.theHandCardVisual.enabled = true;

        transform.SetParent(thehandCardBase.handCardDeck.transform);

        thehandCardBase.index = currentToSlot.index;
        thehandCardBase.slotRectTrans = currentToSlot.transform as RectTransform;


        thehandCardBase.handCardDeck.handCards.Insert(thehandCardBase.index-1, thehandCardBase);
        thehandCardBase.handCardDeck.ResetCardIndex();
        //thehandCardBase.handCardDeck.ResetCardIndexWithSlots();

    }
    #region ugui�¼�

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        theTableCardVisual.toPos = table.dragPoint.position;
        OnDragEvent?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        OnEndDragEvent?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
    #endregion
}
