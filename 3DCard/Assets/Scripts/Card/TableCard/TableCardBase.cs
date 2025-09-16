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
    public Dictionary<CardSO, int> RootCardContains => GetRootCardContains();//如果是堆叠的根节点,则记录所有子节点的CardSO和数量


    //堆叠相关
    public Transform childStackTransform;//这个只是用来设置toPos的,不设置父子关系
    public TableCardBase parentTableCard;
    public TableCardBase childTableCard;
    
    //与拖拽相关
    public bool isDragging = false;
    public UnityAction<TableCardBase> OnDragEvent;
    public UnityAction<TableCardBase> OnEndDragEvent;

    //拖拽到堆叠相关
    public UnityAction<TableCardBase> OnStackToTheTableCardEvent;

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
        OnStackToTheTableCardEvent -= table.tableCardsControl.OnTheTableCardStackToTheTableCard;
    }
    #endregion


    /// <summary>
    /// 只涉及组件的启用和事件监听
    /// </summary>
    protected void Init()
    {
        transform.SetParent(MapMgr.Instance.currentRoom.table.tableCardTransformParent);

        OnEndDragEvent += table.tableCardsControl.OnTheTableCardEndDrag;
        OnDragEvent += table.tableCardsControl.OnTheTableCardDrag;
        OnStackToTheTableCardEvent += table.tableCardsControl.OnTheTableCardStackToTheTableCard;

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

        
        //如果是root节点
        if (parentTableCard == null)
        {
            table.tableCardsControl.tableRootCards.Remove(this);
        }
        

        thehandCardBase.handCardDeck.handCards.Insert(thehandCardBase.index-1, thehandCardBase);
        thehandCardBase.handCardDeck.ResetCardIndex();
        thehandCardBase.handCardDeck.ResetCardSlibing();
        //thehandCardBase.handCardDeck.ResetCardIndexWithSlots();

    }

    public Dictionary<CardSO,int> GetRootCardContains()
    {
        Dictionary<CardSO, int> res = new();
        AddContains(res, this);
        if (childTableCard != null)
        {
            AddContains(res, childTableCard);
        }
        return res;
    }
    private void AddContains(Dictionary<CardSO,int> res,TableCardBase theTableCard)
    {
        if(res.ContainsKey(theTableCard.cardSO))
        {
            res[theTableCard.cardSO]++;
        }
        else
        {
            res[theTableCard.cardSO] = 1;
        }
    }

    /// <summary>
    /// 是否能够拖拽到在子类完成,现在的想法是全部都可以拖拽堆叠
    /// OnStackToTheTableCardEvent在此处触发
    /// </summary>
    /// <param name="toStackTableCard"></param>
    public virtual void TryStackToTheTableCard(TableCardBase toStackTableCard)
    {
        if(IsMyDescendant(toStackTableCard))
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
        MusicMgr.Instance.PlaySound("card#5", transform);
        OnStackToTheTableCardEvent?.Invoke(this);
    }
    /// <summary>
    /// 检查给定的卡牌是否是当前卡牌的子孙卡牌（包括自身）。
    /// </summary>
    /// <param name="potentialDescendant">需要检查的卡牌</param>
    /// <returns>如果是，则返回 true；否则返回 false。</returns>
    private bool IsMyDescendant(TableCardBase potentialDescendant)
    {
        TableCardBase currentCard = this;
        while (currentCard != null)
        {
            if (currentCard == potentialDescendant)
            {
                return true;
            }
            currentCard = currentCard.childTableCard;
        }
        return false;
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
    /// <summary>
    /// 设置自己和所有子节点为最上层
    /// </summary>
    private void SetLastSlibing()
    {
        transform.SetAsLastSibling();
        if(childTableCard != null)
        {
            if (childTableCard == parentTableCard
                || childTableCard == this
                || parentTableCard == this)
            {
                Debug.LogError("TableCardBase 有环");
                return;
            }
            childTableCard.SetLastSlibing();
        }
    }
    /// <summary>
    /// 设置自己和所有子节点不阻挡射线
    /// </summary>
    private void SetNotBlockRaycast()
    {
        canvasGroup.blocksRaycasts = false;
        if (childTableCard != null)
        {
            if (childTableCard == parentTableCard
                || childTableCard == this
                || parentTableCard == this)
            {
                Debug.LogError("TableCardBase 有环");
                return;
            }
            childTableCard.SetNotBlockRaycast();
        }
    }
    /// <summary>
    /// 设置自己和所有子节点阻挡射线
    /// </summary>
    private void SetBlockRaycast()
    {
        canvasGroup.blocksRaycasts = true;
        if (childTableCard != null)
        {
            if (childTableCard == parentTableCard
                || childTableCard == this
                || parentTableCard == this)
            {
                Debug.LogError("TableCardBase 有环");
                return;
            }
            childTableCard.SetBlockRaycast();
        }

    }
    private TableCardBase GetLastTableCard()
    {
        
        if (childTableCard != null)
        {
            if (childTableCard == parentTableCard
                ||childTableCard == this
                ||parentTableCard==this)
            {
                Debug.LogError("TableCardBase 有环");
                return null;
            }
            return childTableCard.GetLastTableCard();
        }
        return this;
    }
    private TableCardBase GetRootTableCard()
    {
        if (parentTableCard != null)
        {
            if (childTableCard == parentTableCard
                || childTableCard == this
                || parentTableCard == this)
            {
                Debug.LogError("TableCardBase 有环");
                return null;
            }
            return parentTableCard.GetRootTableCard();
        }
        return this;
    }
    /// <summary>
    /// 从堆叠链表删除
    /// </summary>
    public void Deleted()
    {
        if(parentTableCard!= null)//设置父亲的child为自己的child
            parentTableCard.childTableCard = childTableCard;
        if (childTableCard != null)
        {
            
            if (parentTableCard == null)
            {
                childTableCard.parentTableCard = null;
                childTableCard.theTableCardVisual.stackPointTransform = null;
                table.tableCardsControl.tableRootCards.Add(childTableCard);
            }
            else
            {
                childTableCard.parentTableCard = parentTableCard;
                childTableCard.theTableCardVisual.stackPointTransform = parentTableCard.childStackTransform;
            }
        }

        if(parentTableCard == null)
        {
            table.tableCardsControl.tableRootCards.Remove(this);
        }

        ClearToInit();
        PoolMgr.Instance.PushObj(this.gameObject);
    }

    public override void ClearToInit()
    {
        thehandCardBase.enabled = true;
        thehandCardBase.theHandCardVisual.enabled = true;
        parentTableCard = null;
        childTableCard = null;

        enabled = false;
        theTableCardVisual.enabled = false;
        theTableCardVisual.stackPointTransform = null;
    }

    #region ugui事件


    public void OnPointerEnter(PointerEventData eventData)
    {
        thehandCardBase.handCardDeck.player.playerInteract.pointCard = GetLastTableCard();
        //MusicMgr.Instance.PlaySound("card#8", transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
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
