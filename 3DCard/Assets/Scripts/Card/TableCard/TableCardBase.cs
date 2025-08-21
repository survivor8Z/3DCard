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
    [HideInInspector]public CanvasGroup canvasGroup; 

    public Table table=>MapMgr.Instance.currentRoom.table;
    [HideInInspector]public TableCardVisual theTableCardVisual;
    [HideInInspector]public HandCardBase thehandCardBase;
    [HideInInspector]public CardView cardView;


    //堆叠相关
    public Transform childStackTransform;//这个只是用来设置toPos的,不设置父子关系
    public TableCardBase parentTableCard;
    public TableCardBase childTableCard;


    //与拖拽相关
    public bool isDragging = false;
    //public UnityAction<TableCardBase> OnBeginDragEvent;
    public UnityAction<TableCardBase> OnDragEvent;
    public UnityAction<TableCardBase> OnEndDragEvent;

    //拖拽到手牌相关
    public Slot toSlot;
    
    private int interactableID;
    public int InteractableID => interactableID;

    public void React()
    {
        
    }

    #region 生命周期函数
    private void Awake()
    {
        thehandCardBase = GetComponent<HandCardBase>();
        canvasGroup = GetComponent<CanvasGroup>();

        theTableCardVisual = GetComponent<TableCardVisual>();
        cardView = GetComponentInChildren<CardView>();
        childStackTransform = transform.Find("CardView/ChildStackTransform");
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


    //只涉及组件的启用和事件监听
    protected void Init()
    {
        transform.SetParent(MapMgr.Instance.currentRoom.table.tableCardTransformParent);

        OnEndDragEvent += table.tableCardsControl.OnTheTableCardEndDrag;
        OnDragEvent += table.tableCardsControl.OnTheTableCardDrag;

        theTableCardVisual.enabled = true;
        theTableCardVisual.Init();
    }

    //#region 拖拽打出相关
    //public virtual void TryDragPlay()
    //{

    //}
    //#endregion

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

    /// <summary>
    /// 在子类完成
    /// </summary>
    /// <param name="toStackTableCard"></param>
    public virtual void TryStackToTheTableCard(TableCardBase toStackTableCard)
    {
        if(toStackTableCard == this)
        {
            return;
        }
        //默认成功
        if (table.tableCardsControl.tableRootCards.Contains(this))
        {
            table.tableCardsControl.tableRootCards.Remove(this);
        }
        toStackTableCard.childTableCard = this;
        this.parentTableCard = toStackTableCard;
        theTableCardVisual.stackPointTransform = toStackTableCard.childStackTransform;

    }

    public void ToRootTableCard()
    {
        theTableCardVisual.stackPointTransform = null;
        //解除自己与自己的父亲的关系
        if (parentTableCard != null)//解除父子关系在子这里做
        {
            parentTableCard.childTableCard = null;
            parentTableCard = null;
        }
        if (!table.tableCardsControl.tableRootCards.Contains(this))
        {
            table.tableCardsControl.tableRootCards.Add(this);
        }
    }

    /// <summary>
    /// 回到桌牌的初始状态
    /// </summary>
    public void ResetTableCardState()
    {
        theTableCardVisual.offsetDragAboveK = 0f;
        isDragging = false;
        theTableCardVisual.stackPointTransform = null;
        if (parentTableCard != null)//解除父子关系在子这里做
        {
            parentTableCard.childTableCard = null;
            parentTableCard = null;
        }
        if (!table.tableCardsControl.tableRootCards.Contains(this))
        {
            table.tableCardsControl.tableRootCards.Add(this);
        }
    }

    private void SetLastSlibing()
    {
        //print("SetLastSlibing");
        transform.SetAsLastSibling();
        if(childTableCard != null)
        {
            childTableCard.SetLastSlibing();
        }
    }

    private void SetNotBlockRaycast()
    {
        canvasGroup.blocksRaycasts = false;
        if (childTableCard != null)
        {
            childTableCard.SetNotBlockRaycast();
        }
    }
    private void SetBlockRaycast()
    {
        canvasGroup.blocksRaycasts = true;
        if (childTableCard != null)
        {
            childTableCard.SetBlockRaycast();
        }

    }
    private TableCardBase GetLastTableCard()
    {
        if (childTableCard != null)
        {
            return childTableCard.GetLastTableCard();
        }
        return this;
    }
    private TableCardBase GetRootTableCard()
    {
        if (parentTableCard != null)
        {
            return parentTableCard.GetRootTableCard();
        }
        return this;
    }

    #region ugui事件


    public void OnPointerEnter(PointerEventData eventData)
    {
        //print("OnPointerEnter TableCardBase");
        thehandCardBase.handCardDeck.player.playerInteract.pointCard = GetLastTableCard();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //print("OnPointerExit TableCardBase");
        thehandCardBase.handCardDeck.player.playerInteract.pointCard = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        theTableCardVisual.offsetDragAboveK = theTableCardVisual.offsetDragAbove;
        SetNotBlockRaycast();
        ToRootTableCard();
        SetLastSlibing();

        if (thehandCardBase.handCardDeck.player.playerInteract.pointCard as TableCardBase == this)
        {
            thehandCardBase.handCardDeck.player.playerInteract.pointCard = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        theTableCardVisual.toPos = table.dragPoint.position;
        theTableCardVisual.offsetDragAboveK = theTableCardVisual.offsetDragAbove;   
        OnDragEvent?.Invoke(this);
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        theTableCardVisual.offsetDragAboveK = 0f;
        OnEndDragEvent?.Invoke(this);
        SetBlockRaycast();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    #endregion

    
}
