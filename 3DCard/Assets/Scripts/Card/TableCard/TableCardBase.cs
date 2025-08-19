using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TableCardBase : CardBase,IInteractable
    , IPointerEnterHandler
    , IPointerExitHandler
    , IDragHandler
    , IBeginDragHandler
    , IEndDragHandler
    , IPointerUpHandler
    , IPointerDownHandler
{
    public Table table=>MapMgr.Instance.currentRoom.table;
    [HideInInspector]public TableCardVisual theTableCardVisual;
    [HideInInspector]public HandCardBase thehandCardBase;


    //�ѵ����
    public Vector3 offset = new Vector3(1, 0, 0);
    public float offsetK = 0.2f;


    //����ק���
    public bool isDragging = false;
    //public UnityAction<TableCardBase> OnBeginDragEvent;
    public UnityAction<TableCardBase> OnDragEvent;
    public UnityAction<TableCardBase> OnEndDragEvent;

    //��ק���������
    public Slot toSlot;
    
    private int interactableID;
    public int InteractableID => interactableID;

    public void React()
    {
        
    }

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

    //ֻ�漰��������ú��¼�����
    protected void Init()
    {
        transform.SetParent(MapMgr.Instance.currentRoom.table.tableCardTransformParent);

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
        thehandCardBase.handCardDeck.ResetCardSlibing();
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

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="theTableCard"></param>
    public virtual void TryStackToTheTableCard(TableCardBase theTableCard)
    {
        
        
    }
}
