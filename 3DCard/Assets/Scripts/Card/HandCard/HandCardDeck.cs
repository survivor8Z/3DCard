using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
/// <summary>
/// 负责手牌管理:增删手牌,设置整体位置
/// </summary>

public class HandCardDeck : SerializedMonoBehaviour
{
    public Player player;
    public InteractableObject currentPointInteractableObject;//当前指向的可交互物体

    public int maxCount = 10;
    public List<HandCardBase> handCards = new List<HandCardBase>();
    public Transform circleCenter;
    public HandCardBase hoveredCard;
    public HandCardBase dragedCard;//不是卡牌被拖拽,会有线指示
    public HandCardBase selectedCard;
    public int CurrentHandCardCount => handCards.Count;

    public Vector2 mousePositionViewport;
   

    private RectTransform theRectTransform;

    public HandCardDeckVisual handCardDeckVisual;
    
    public Slots slots;//槽位盘
    [SerializeField]private GameObject slotPre;

    public List<CardSO> cardSOs = new List<CardSO>(); //为了在编辑器里搞,awake时加到cardPreDic里
    public List<GameObject> cardPreList = new List<GameObject>(); //为了在编辑器里搞,awake时加到cardPreDic里
    [ShowInInspector]private Dictionary<CardSO,GameObject> cardPreDic = new();

    //用于取消
    public bool isCancel = false;

    //用于与桌牌转换
    public Table table;

    //用于创建预槽位
    public int currentIndex;


    #region 生命周期函数
    private void Awake()
    {
        for(int i= 0; i < cardSOs.Count; i++)
        {
            cardPreDic.Add(cardSOs[i], cardPreList[i]);
        }


        theRectTransform = GetComponent<RectTransform>();
        handCardDeckVisual = GetComponent<HandCardDeckVisual>();
    }
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardPointUp, OnHandCardClick);
    }

    public CardSO testCardSO;
    private void Update()
    {
        
        //test
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddCard(testCardSO);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            DelTheCard(1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ResetCardState();
        }

    }
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardPointUp, OnHandCardClick);
    }
    private void OnDestroy()
    {
        
    }

    #endregion

    #region 实际卡牌打出效果相关
    private Dictionary<CardCombinationPlayID, int> _combinationFuncDic;
    public IReadOnlyDictionary<CardCombinationPlayID, int> CombinationFuncDic
    {
        get
        {
            _combinationFuncDic ??= new Dictionary<CardCombinationPlayID, int>();
            return _combinationFuncDic;
        }
        protected set
        {
            _combinationFuncDic = value != null
                ? new Dictionary<CardCombinationPlayID, int>(value)
                : new Dictionary<CardCombinationPlayID, int>();
        }
    }

    protected void AddCombinationFuncDic(CardCombinationPlayID combinationPlayID, int funcID)
    {
        if (_combinationFuncDic.ContainsKey(combinationPlayID))
        {
            Debug.LogWarning($"DragedFuncDic already contains {combinationPlayID}, updating value.");
            _combinationFuncDic[combinationPlayID] = funcID;
            return;
        }
        _combinationFuncDic[combinationPlayID] = funcID;
    }

    protected void RemoveCombinationFuncDic(CardCombinationPlayID combinationPlayID)
    {
        if (_combinationFuncDic.ContainsKey(combinationPlayID))
        {
            _combinationFuncDic.Remove(combinationPlayID);
        }
        else
        {
            Debug.LogWarning($"DragedFuncDic does not contain {combinationPlayID}, cannot remove.");
        }
    }


    private bool CanCardCombinationPlay(CardCombinationPlayID cardCombinationPlayID)
    {
        if (CombinationFuncDic.ContainsKey(cardCombinationPlayID))
            return true;
        return false;
    }

    /// <summary>
    /// 给PlayerInteract调用
    /// </summary>
    public void TryCardCombinationPlay(CardCombinationPlayID cardCombinationPlayID)
    {
        if (CanCardCombinationPlay(cardCombinationPlayID))
        {
            //执行打出
        }
        else
        {
            //执行不能打出(特效音效等等)
        }

        Debug.Log("TryCardCombinationPlay");
        selectedCard.isSelected = false; // 清除选中状态
        selectedCard = null;
        dragedCard = null;
    }
    #endregion
    

    /// <summary>
    /// 添加一张手牌,一般是与interactableObject交互添加
    /// </summary>
    /// <param name="cardSO"></param>
    public void AddCard(CardSO cardSO)
    {
        if (handCards.Count >= maxCount)
        {
            Debug.Log("HandCardDeck: AddCard: Hand card count exceeds max limit");
            return;
        }
        GameObject slotObj = Instantiate(slotPre, slots.transform);
        slotObj.name = "Slot" + (slots.transform.childCount);
        GameObject card = Instantiate(cardPreDic[cardSO], transform);
        card.name = "Card" + (handCards.Count + 1);

        HandCardBase theHandCard = card.GetComponent<HandCardBase>();
        Slot theSlot = slotObj.GetComponent<Slot>();

        handCards.Add(theHandCard);

        slots.slotsList.Add(theSlot);
        theSlot.index = handCards.Count;

        theHandCard.cardSO = cardSO; // 设置卡牌数据
        theHandCard.handCardDeck = this;
        theHandCard.slotRectTrans = slotObj.GetComponent<RectTransform>();
        theHandCard.index = handCards.Count;

    }
    /// <summary>
    /// 提前就会创建一个slot,能否创建也已经在之前判断了
    /// 其他地方拿,不会删除(隐藏)那个gameobject,其他与AddCard一样
    /// </summary>
    /// <param name="cardSO"></param>
    public void AddCardWithTableCard(TableCardBase tableCardBase)
    {
        GameObject card = tableCardBase.gameObject;
        card.name = "Card" + (handCards.Count + 1);
        //TODO:等写TableCardToHandCard的时候在说把,没写完
        card.transform.SetParent(transform);


    }
    

    /// <summary>
    /// 插入一个Slot,返回其引用
    /// </summary>
    public Slot InsertASlot(int listIndex)
    {
        if (handCards.Count >= maxCount)
        {
            Debug.Log("Hand card count exceeds max limit");
            return null;
        }
        GameObject slotObj = Instantiate(slotPre, slots.transform);
        Slot theSlot = slotObj.GetComponent<Slot>();

        slots.slotsList.Insert(listIndex,theSlot);
        slotObj.transform.SetSiblingIndex(listIndex);//
        ResetSlotsIndex();
        return theSlot;
    }

    public void SetTheSlotPos(Slot slot,int insertSlotListIndex)
    {
        if(insertSlotListIndex == slots.slotsList.Count)
        {
            Debug.Log("SetTheSlotPos: insertSlotListIndex out of range");
            return;
        }

        slot.transform.SetSiblingIndex(insertSlotListIndex);

        (slots.slotsList[slot.index - 1], slots.slotsList[insertSlotListIndex])
            = (slots.slotsList[insertSlotListIndex], slots.slotsList[slot.index - 1]);
        //slots.slotsList.RemoveAt(slot.index - 1);
        //slots.slotsList.Insert(insertSlotListIndex, slot);
        ResetSlotsIndex();
    }

    /// <summary>
    /// 移除一个Slot,这个slot是没有卡牌的,如果有卡牌,不调用这个方法
    /// </summary>
    /// <param name="slot"></param>
    public void RemoveTheSlot(Slot slot)
    {
        slots.slotsList.RemoveAt(slot.index - 1);
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardSlotDel, slot.index);
        ResetSlotsIndex();
    }

    /// <summary>
    /// index:手牌的索引,从1开始
    /// </summary>
    /// <param name="index"></param>
    public void DelTheCard(int index)
    {
        if(index <=0 || index > handCards.Count)
        {
            Debug.Log("DelTheCard: index out of range");
            return;
        }
        handCards.RemoveAt(index - 1);
        slots.slotsList.RemoveAt(index - 1);
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardDel, index);//这个只是通知HandCardBase删除(虽然说我觉得很冗余)
        ResetCardIndex();
    }

    /// <summary>
    /// index:手牌的索引,从1开始
    /// 和AddCard差不多,但是不移除
    /// </summary>
    /// <param name="index"></param>
    public void TheCardLeaveHandDeck(int index)
    {
        if (index <= 0 || index > handCards.Count)
        {
            Debug.Log("DelTheCard: index out of range");
            return;
        }
        handCards.RemoveAt(index - 1);
        slots.slotsList.RemoveAt(index - 1);
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardLeaveHandCardDeck, index);
        ResetCardIndex();
    }

    public void ResetSlotsIndex()
    {
        for(int i = 0;i < slots.slotsList.Count; i++)
        {
            slots.slotsList[i].index = i + 1;
            slots.slotsList[i].name = "Slot" + (i + 1);
        }   
    }

    public void ResetCardIndex()
    {
        for(int i = 0; i < handCards.Count; i++)
        {
            handCards[i].index = i + 1;
            handCards[i].name = "Card" + (i + 1);
            slots.slotsList[i].index = i + 1;
            slots.slotsList[i].name = "Slot" + (i+1);
        }
    }

    public void ResetCardIndexWithSlots()
    {
        if (slots.slotsList.Count != handCards.Count)
        {
            print("ResetCardIndexWithSlot: slots count not equal handCards count");
            return;
        }
        for(int i = 0; i < slots.slotsList.Count; i++)
        {
            slots.slotsList[i].index = i + 1;
            slots.slotsList[i].name = "Slot" + (i + 1);
        }
    }


    public void ResetCardState()
    {
        if (selectedCard != null)
        {
            selectedCard.ResetState();
            selectedCard = null;
        }
        if (dragedCard != null)
        {
            dragedCard.ResetState();
            dragedCard = null;
        }
    }


    #region 事件响应

    public void OnHandCardClick(int index)
    {
        if(isCancel)return;
        if (index <= 0 || index > handCards.Count) return;
        HandCardBase theHandCard = handCards[index - 1];
        if (theHandCard == selectedCard&&theHandCard.isSelected)
        {
            
            theHandCard.isSelected = false; // 清除选中状态+

            selectedCard.transform.SetParent(transform);
            selectedCard = null; // 清除选中引用
        }
        else
        {
            if (selectedCard != null)
            {
                selectedCard.isSelected = false;
            }

            theHandCard.isSelected = true; // 设置为选中状态
            selectedCard = theHandCard; // 更新选中引用

            
            EventCenter.Instance.EventTrigger(E_EventType.E_HandCardSelected, index);
            
            
            

            //更新slot
            Slot theSlot = slots.slotsList[index - 1];
            slots.slotsList.RemoveAt(index - 1);
            slots.slotsList.Add(theSlot);

            handCards.RemoveAt(index - 1);
            handCards.Add(theHandCard);
            
            ResetCardIndex();
            slots.ResetSlots();

        }


    }


    #endregion


    //输入函数
    void OnMousePosition(InputValue value)
    {
        mousePositionViewport = new Vector2(value.Get<Vector2>().x / Screen.width, value.Get<Vector2>().y / Screen.height);
    }
}
