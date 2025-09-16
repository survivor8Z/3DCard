using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用来管理桌牌的
/// </summary>
public class TableCardsControl : MonoBehaviour
{
    public List<TableCardBase> tableRootCards = new List<TableCardBase>();
    public TableCardBase currentDragCard;

    public HandCardDeck handCardDeck=>Player.Instance.playerInteract.handCardDeck;
    public PlayerInteract playerInteract => handCardDeck.player.playerInteract;

    //放回相关
    public float toHandY;

    public Table table;

    #region 生命周期函数
    private void Awake()
    {
        table = GetComponent<Table>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TickAllCombinationCreate();
        }
    }
    #endregion

    #region 放回手牌相关
    /// <summary>
    /// 用于确认将要把桌牌放到手牌中的哪个槽位,index是listindex
    /// 0<=res<=handCardDeck.slots.slotsList.Count
    /// </summary>
    /// <param name="MouseScreenPosition"></param>
    /// <returns></returns>
    public int AboutToCreateSlotIndex(Vector2 MouseScreenPosition)
    {
        int res = 0;
        while (res < handCardDeck.slots.slotsList.Count
               && Camera.main.WorldToScreenPoint(handCardDeck.slots.slotsList[res].transform.position).x < MouseScreenPosition.x)
        {
            res++;
        }
        return res;
    }

    /// <summary>
    /// 这个是已经创建了一个slot了
    /// 用于确认将要把桌牌放到手牌中的哪个槽位,index是listindex
    /// 0<=res<handCardDeck.slots.slotsList.Count
    /// </summary>
    /// <param name="MouseScreenPosition"></param>
    /// <returns></returns>
    public int AboutToSetSlotIndex(Vector2 MouseScreenPosition)
    {
        int res = 0;
        while(res < handCardDeck.slots.slotsList.Count-1
               && Camera.main.WorldToScreenPoint(handCardDeck.slots.slotsList[res].transform.position).x < MouseScreenPosition.x)
        {
            res++;
        }
        return res;
    }
    
    Slot currentToSlot;
    /// <summary>
    /// 尝试插入slot,
    /// </summary>
    public void TryInsertSlot(Vector2 MouseScreenPosition)
    {
        if (handCardDeck.maxCount <= handCardDeck.handCards.Count
            || MouseScreenPosition.y / Screen.height >= toHandY
            || currentDragCard.childTableCard!=null /*有子牌也不插入slot*/)
        {
            //执行放回失败逻辑,如果有的话,如果这次拖拽插入了还要删除
            if(currentToSlot != null)
            {
                handCardDeck.RemoveTheSlot(currentToSlot);
                currentToSlot = null;
            }
            currentDragCard.toSlot = null;

            return;
        }
        
        if (currentToSlot == null)
        {
            //这个index是list的index
            int insertSlotListIndex = AboutToCreateSlotIndex(MouseScreenPosition);
            //print("insertSlotListIndex" + insertSlotListIndex);
            currentToSlot = handCardDeck.InsertASlot(insertSlotListIndex);

        }
        else
        {
            int setSlotListIndex = AboutToSetSlotIndex(MouseScreenPosition);
            //print("setSlotListIndex " + setSlotListIndex);
            if (setSlotListIndex != currentToSlot.index-1)
            {
                handCardDeck.SetTheSlotPos(currentToSlot, setSlotListIndex);
            }
        }
        currentDragCard.toSlot = currentToSlot;


    }
    #endregion
    /// <summary>
    /// 递归获取桌牌的根节点
    /// </summary>
    /// <param name="theTableCard"></param>
    /// <returns></returns>
    public TableCardBase GetRootCard(TableCardBase theTableCard)
    {
        if(theTableCard.parentTableCard == null)
        {
            return theTableCard;
        }
        else
        {
            return GetRootCard(theTableCard.parentTableCard);
        }
    }

    /// <summary>
    /// 创建一个桌牌
    /// 可能没初始化完全
    /// </summary>
    /// <param name="cardSO"></param>
    /// <param name="theRootCard"></param>
    public void CreateTableCard(CardSO cardSO,TableCardBase theRootCard)
    {
        GameObject card = PoolMgr.Instance.GetObjSync("HandCard_" + cardSO.cardEnglishName);

        card.transform.position = theRootCard.transform.position;
        card.transform.SetParent(transform);
        card.transform.localScale = Vector3.one;
        card.transform.rotation = table.dragPoint.rotation;

        HandCardBase theHandCard = card.GetComponent<HandCardBase>();
        TableCardBase tableCardBase = card.GetComponent<TableCardBase>();
        tableCardBase.enabled = true;
        tableCardBase.theTableCardVisual.enabled = true;
        //随机在theRootCard旁边随机偏移一个位置
        tableCardBase.theTableCardVisual.toPos
            = theRootCard.transform.position + new Vector3(Random.Range(-0.15f, 0.15f), 0, Random.Range(-0.15f, 0.15f));

        theHandCard.cardSO = cardSO; // 设置卡牌数据
        theHandCard.handCardDeck = handCardDeck;
        
        theHandCard.enabled = false;
        theHandCard.theHandCardVisual.enabled = false;

        tableRootCards.Add(theHandCard.theTableCardBase);
    }
    public void TryCreateTableCardByCombination(TableCardBase theRootCard)
    {
        (CardSO toCreateCard,List<TableCardBase> materialCards)
            = GetCombinationMaterialCardsAndToCreateCard(theRootCard);
        if (toCreateCard == null || materialCards == null)
        {
            Debug.Log("没有符合的合成");

            return;
        }
        //删除材料
        foreach (var materialCard in materialCards)
        {
            materialCard.Deleted();
        }
        //创建新卡牌
        CreateTableCard(toCreateCard, theRootCard);

    }
    /// <summary>
    /// 尝试对所有的桌牌进行合成,一般是过房间时调用
    /// </summary>
    public void TickAllCombinationCreate()
    {
        // 创建 tableRootCards 的一个副本进行遍历
        List<TableCardBase> rootsToProcess = new List<TableCardBase>(tableRootCards);

        foreach (var theRootCard in rootsToProcess)
        {
            // 尝试合成，这个方法会修改 tableRootCards 列表
            TryCreateTableCardByCombination(theRootCard);
        }
    }

    // 根据合成表和优先级，获取合成材料以及结果卡牌
    private (CardSO, List<TableCardBase>) GetCombinationMaterialCardsAndToCreateCard(TableCardBase theRootCard)
    {
        if (theRootCard == null) return default;

        // 1. 只进行一次卡牌堆统计
        Dictionary<CardSO, int> stackCounts = CountCardsInStack(theRootCard);

        // 2. 遍历已排序的组合规则列表
        for (int i = 0; i < TableCardCombinationMgr.Instance.CombinationRuleList.Count; i++)
        {
            CardCombinationRule cardCR = TableCardCombinationMgr.Instance.CombinationRuleList[i];
            // 3. 检查卡牌堆是否满足当前规则
            if (CheckTheCombination(cardCR, stackCounts))
            {
                // 如果匹配成功，则找出具体的材料卡牌
                List<TableCardBase> materialCards = new List<TableCardBase>();

                // 复制一份统计结果，用于在查找材料时进行消耗计数
                Dictionary<CardSO, int> tempRequiredCounts = cardCR.requiredCards
                    .GroupBy(card => card)
                    .ToDictionary(g => g.Key, g => g.Count());

                TableCardBase currentCard = theRootCard;
                while (currentCard != null)
                {
                    // 如果当前卡牌是规则所需的材料，且数量还未用完
                    if (tempRequiredCounts.ContainsKey(currentCard.cardSO) && tempRequiredCounts[currentCard.cardSO] > 0)
                    {
                        materialCards.Add(currentCard);
                        tempRequiredCounts[currentCard.cardSO]--;
                    }
                    currentCard = currentCard.childTableCard;
                }

                // 返回结果卡牌和材料列表
                return (cardCR.resultCard, materialCards);
            }
        }

        // 没有找到任何匹配的组合
        return default;
    }
    /// 递归地统计一个卡牌堆中所有卡牌的数量。
    /// </summary>
    private Dictionary<CardSO, int> CountCardsInStack(TableCardBase rootCard)
    {
        var cardCounts = new Dictionary<CardSO, int>();
        if (rootCard == null)
        {
            return cardCounts;
        }

        TableCardBase currentCard = rootCard;
        while (currentCard != null)
        {
            if (cardCounts.ContainsKey(currentCard.cardSO))
            {
                cardCounts[currentCard.cardSO]++;
            }
            else
            {
                cardCounts[currentCard.cardSO] = 1;
            }
            currentCard = currentCard.childTableCard;
        }
        return cardCounts;
    }

    public void DebugLogCardStack(TableCardBase rootCard)
    {
        string stackString = "Card Stack: ";
        TableCardBase currentCard = rootCard;
        while (currentCard != null)
        {
            stackString += currentCard.cardSO.cardEnglishName + " -> ";
            currentCard = currentCard.childTableCard;
        }
        Debug.Log(stackString + "END");
    }

    private void SetMaterialCardsList(List<TableCardBase> materialCards, CardCombinationRule cardCR, TableCardBase theRootCard)
    {
        Dictionary<CardSO, int> cardCRDic = new();
        for (int i = 0; i < cardCR.requiredCards.Count; i++)
        {
            if (!cardCRDic.ContainsKey(cardCR.requiredCards[i]))
            {
                cardCRDic.Add(cardCR.requiredCards[i], 1);
            }
            else
            {
                cardCRDic[cardCR.requiredCards[i]]++;
            }
        }
        if (theRootCard != null)
        {
            if (cardCRDic[theRootCard.cardSO]>=1)
            {
                materialCards.Add(theRootCard);
                cardCRDic[theRootCard.cardSO]--;
            }
        }
        SetMaterialCardsList(materialCards, cardCR, theRootCard.childTableCard);
    }

    /// <summary>
    /// 检查卡牌堆是否包含一个组合规则所需的所有卡牌。
    /// </summary>
    private bool CheckTheCombination(CardCombinationRule rule, Dictionary<CardSO, int> stackCards)
    {
        // 1. 统计规则所需的卡牌数量
        var requiredCounts = new Dictionary<CardSO, int>();
        foreach (var card in rule.requiredCards)
        {
            if (requiredCounts.ContainsKey(card))
            {
                requiredCounts[card]++;
            }
            else
            {
                requiredCounts[card] = 1;
            }
        }

        // 2. 比对规则需求和卡牌堆中的卡牌数量
        foreach (var requiredPair in requiredCounts)
        {
            // 如果卡牌堆中没有这种卡牌，或者数量不足，则返回 false
            if (!stackCards.ContainsKey(requiredPair.Key) || stackCards[requiredPair.Key] < requiredPair.Value)
            {
                return false; // 该组合无法合成
            }
        }

        return true; // 所有需求都满足
    }

    // GUI 调试变量
    private bool showDebugPanel = true;
    private void OnGUI()
    {
        if((MapMgr.Instance.WorldPosToRoomWorldCoorBig(transform.position)
                 != MapMgr.Instance.WorldPosToRoomWorldCoorBig(Player.Instance.transform.position)))//不在同一房间内
        {
            return;
        }
        GUILayout.BeginArea(new Rect(260, 10, 250, 500));

        showDebugPanel = GUILayout.Toggle(showDebugPanel, showDebugPanel ? "隐藏桌牌调试面板" : "显示桌牌调试面板", "Button");

        if (showDebugPanel)
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label("卡牌操作", new GUIStyle { fontStyle = FontStyle.Bold });

            GUILayout.Label("桌牌根节点数量:" + tableRootCards.Count);

            if (GUILayout.Button("尝试对所有桌牌进行合成"))
            {
                TickAllCombinationCreate();
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndArea();
    }


    #region 事件响应
    public void OnTheTableCardDrag(TableCardBase theTableCard)
    {
        currentDragCard = theTableCard;
        TryInsertSlot(Input.mousePosition);
    }

    
    public void OnTheTableCardEndDrag(TableCardBase theTableCard)
    {
        //如果插入了slot,这个就是直接执行转换了

        if(currentDragCard != null&&currentToSlot!=null)
        {
            if(currentDragCard.childTableCard != null)
            {
                return; //有子节点就不允许放回手牌
            }

            //开始正式转换桌牌到手牌
            currentDragCard.TranslateToHandCard(currentToSlot);
        }

        else if(currentDragCard!=null && currentToSlot == null)//这个就是桌牌触发相关了
        {
            //print("UGUI事件触发");
            if (playerInteract.pointCard is TableCardBase toStackTableCard)
            {
                currentDragCard.TryStackToTheTableCard(toStackTableCard);
            }else if(playerInteract.pointInteractableObject is Table)
            {
                currentDragCard.ToRootTableCard();
                MusicMgr.Instance.PlaySound("card#1", currentDragCard.transform);
            }
        }


        currentToSlot = null; //清空当前的slot
        currentDragCard = null; 
    }
    //这个主要是处理堆叠后显示合成的卡牌,触发音效
    public void OnTheTableCardStackToTheTableCard(TableCardBase theTableCard)
    {
        TableCardBase theRootCard = GetRootCard(theTableCard);

    }
    #endregion

}
