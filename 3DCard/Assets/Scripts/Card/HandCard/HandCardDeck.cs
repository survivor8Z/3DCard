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
    [SerializeField] private GameObject slotPre;

    //用于取消
    public bool isCancel = false;

    //用于与桌牌转换
    public Table table => MapMgr.Instance.currentRoom.table;

    //用于创建预槽位
    public int currentIndex;


    #region 生命周期函数
    private void Awake()
    {
        theRectTransform = GetComponent<RectTransform>();
        handCardDeckVisual = GetComponent<HandCardDeckVisual>();
    }
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardPointUp, OnHandCardClick);
        EventCenter.Instance.AddEventListener(E_EventType.E_HandCardCancel, ResetCardState);
    }

    private void Update()
    {
        //实时创建Entity预览
        if (selectedCard != null
            && selectedCard is HandCard_Entity handCard_Entity)
        {
            if (handCard_Entity.toCreateEntity != null)
            {
                handCard_Entity.TryCreateEntityInGround();
                if (!handCardDeckVisual.isHide)
                    handCard_Entity.DelPreViewEntity();
            }
            else
            {
                if (handCardDeckVisual.isHide)
                    handCard_Entity.CreatePreviewEntity();
            }
        }




        //test
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddressablesMgr.Instance.LoadAssetCoroutine<CardSO>("防御", (SO) =>
            {
                AddCard(SO.Result);
            });
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddressablesMgr.Instance.LoadAssetCoroutine<CardSO>("石头", (SO) =>
            {
                AddCard(SO.Result);
            });
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            DelTheCard(1);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ResetCardState();
        }
    }

    private bool showGUI = false;
    private string inputStr = "";




    // GUI 调试变量
    private bool showDebugPanel = true;
    private string cardNameInput = "";
    private string removeIndexInput = "1";
    private void OnGUI()
    {
        // 使用 GUILayout 自动布局生成调试按钮
        GUILayout.BeginArea(new Rect(10, 10, 250, 500));

        // 创建一个可折叠的面板
        showDebugPanel = GUILayout.Toggle(showDebugPanel, showDebugPanel ? "隐藏手牌调试面板" : "显示手牌调试面板", "Button");

        if (showDebugPanel)
        {
            GUILayout.BeginVertical("Box");

            GUILayout.Label("卡牌操作", new GUIStyle { fontStyle = FontStyle.Bold });

            // 添加卡牌区域
            GUILayout.BeginHorizontal();
            GUILayout.Label("添加卡牌:", GUILayout.Width(70));
            cardNameInput = GUILayout.TextField(cardNameInput, GUILayout.Width(100));
            if (GUILayout.Button("添加"))
            {
                if (!string.IsNullOrEmpty(cardNameInput))
                {
                    AddressablesMgr.Instance.LoadAssetCoroutine<CardSO>(cardNameInput, (SO) =>
                    {
                        if (SO.Result != null)
                        {
                            AddCard(SO.Result);
                            Debug.Log($"成功添加卡牌: {SO.Result.cardName}");
                        }
                        else
                        {
                            Debug.LogError($"无法找到名为 '{cardNameInput}' 的卡牌。");
                        }
                    });
                }
            }
            GUILayout.EndHorizontal();

            // 删除卡牌区域
            GUILayout.BeginHorizontal();
            GUILayout.Label("删除索引:", GUILayout.Width(70));
            removeIndexInput = GUILayout.TextField(removeIndexInput, GUILayout.Width(100));
            if (GUILayout.Button("删除"))
            {
                if (int.TryParse(removeIndexInput, out int index))
                {
                    DelTheCard(index);
                }
            }
            GUILayout.EndHorizontal();

            // 一些预设的快速测试按钮
            GUILayout.Space(10);
            if (GUILayout.Button("快速添加 '打击' 卡牌 "))
            {
                AddressablesMgr.Instance.LoadAssetCoroutine<CardSO>("打击", (SO) => AddCard(SO.Result));
            }
            if (GUILayout.Button("快速添加 '木头' 卡牌 "))
            {
                AddressablesMgr.Instance.LoadAssetCoroutine<CardSO>("木头", (SO) => AddCard(SO.Result));
            }
            if (GUILayout.Button("快速添加 '石头' 卡牌 "))
            {
                AddressablesMgr.Instance.LoadAssetCoroutine<CardSO>("石头", (SO) => AddCard(SO.Result));
            }
            if (GUILayout.Button("删除第一张卡牌 (J)"))
            {
                DelTheCard(1);
            }
            if (GUILayout.Button("重置卡牌状态 (P)"))
            {
                ResetCardState();
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndArea();
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardPointUp, OnHandCardClick);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_HandCardCancel, ResetCardState);
    }
    private void OnDestroy()
    {

    }

    #endregion

    #region 实际卡牌打出效果相关
    /// <summary>
    /// 给PlayerInteract调用
    /// </summary>
    public void TryCardCombinationPlay(IInteractable pointInteractableObject)
    {
        print("组合打出");
        switch (dragedCard)
        {
            case IAttack iattack:
                if (pointInteractableObject is IDamageable damageable)
                {
                    if (selectedCard is IAddDamage addDamage)
                    {
                        iattack.Attack(damageable);
                    }
                    else
                    {
                        FailCombinationPlay();
                    }
                }
                else
                {
                    FailCombinationPlay();
                }
                break;
            default:
                FailCombinationPlay();
                break;
        }


        selectedCard.isSelected = false; // 清除选中状态
        selectedCard = null;
        dragedCard = null;
    }






    public void FailCombinationPlay()
    {
        print("FailCombinationPlay");
    }
    #endregion

    #region 增删卡牌/槽位,设置卡牌index,状态
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

        GameObject slotObj = PoolMgr.Instance.GetObjSync("Slot");
        slotObj.transform.SetParent(slots.transform);
        slotObj.transform.localPosition = Vector3.zero;
        slotObj.transform.localScale = Vector3.one;

        GameObject card = PoolMgr.Instance.GetObjSync("HandCard_" + cardSO.cardEnglishName);//这里的名字固定,之后桌牌也是HandCard

        card.transform.position = slots.transform.position;
        card.transform.SetParent(transform);
        card.transform.localScale = Vector3.one;

        TableCardBase tableCardBase = card.GetComponent<TableCardBase>();
        TableCardVisual tableCardVisual = card.GetComponent<TableCardVisual>();
        tableCardBase.enabled = false;
        tableCardVisual.enabled = false;

        HandCardBase theHandCard = card.GetComponent<HandCardBase>();
        HandCardVisual handCardVisual = card.GetComponent<HandCardVisual>();
        Slot theSlot = slotObj.GetComponent<Slot>();

        handCards.Add(theHandCard);

        slots.slotsList.Add(theSlot);
        theSlot.index = handCards.Count;

        theHandCard.cardSO = cardSO; // 设置卡牌数据
        theHandCard.handCardDeck = this;
        theHandCard.slotRectTrans = slotObj.GetComponent<RectTransform>();
        theHandCard.index = handCards.Count;
        theHandCard.enabled = true;

        handCardVisual.enabled = true;
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
        //GameObject slotObj = Instantiate(slotPre, slots.transform);
        GameObject slotObj = PoolMgr.Instance.GetObjSync("Slot");
        slotObj.transform.SetParent(slots.transform);
        slotObj.transform.localPosition = Vector3.zero;
        slotObj.transform.localScale = Vector3.one;
        Slot theSlot = slotObj.GetComponent<Slot>();

        slots.slotsList.Insert(listIndex, theSlot);
        slotObj.transform.SetSiblingIndex(listIndex);//
        ResetSlotsIndex();
        return theSlot;
    }

    public void SetTheSlotPos(Slot slot, int insertSlotListIndex)
    {
        if (insertSlotListIndex == slots.slotsList.Count)
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
        if (index <= 0 || index > handCards.Count)
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
        for (int i = 0; i < slots.slotsList.Count; i++)
        {
            slots.slotsList[i].index = i + 1;
            slots.slotsList[i].layoutElement.flexibleWidth = 1f;
        }
    }

    public void ResetCardIndex()
    {
        for (int i = 0; i < handCards.Count; i++)
        {
            handCards[i].index = i + 1;
            slots.slotsList[i].index = i + 1;
            slots.slotsList[i].layoutElement.flexibleWidth = 1f;
        }
    }
    public void ResetCardSlibing()
    {
        for (int i = 0; i < handCards.Count; i++)
        {
            handCards[i].transform.SetSiblingIndex(i + 1);
            slots.slotsList[i].transform.SetSiblingIndex(i);
            slots.slotsList[i].layoutElement.flexibleWidth = 1f;
        }
    }


    public void ResetCardState()
    {
        //print("ResetCardState");
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
    #endregion

    #region 事件响应

    public void OnHandCardClick(int index)
    {
        if (isCancel) return;
        if (index <= 0 || index > handCards.Count) return;
        HandCardBase theHandCard = handCards[index - 1];
        if (theHandCard == selectedCard && theHandCard.isSelected)
        {

            theHandCard.isSelected = false; // 清除选中状态+

            selectedCard.transform.SetParent(transform);
            selectedCard = null; // 清除选中引用
            if (selectedCard is HandCard_Entity handCard_Entity)
            {
                handCard_Entity.DelPreViewEntity();
            }
        }
        else
        {
            if (selectedCard != null)
            {
                selectedCard.isSelected = false;
                if (selectedCard is HandCard_Entity selectedCardHandCard_Entity)
                {
                    selectedCardHandCard_Entity.DelPreViewEntity();//删除之前的预览
                }
            }

            theHandCard.isSelected = true; // 设置为选中状态
            selectedCard = theHandCard; // 更新选中引用


            EventCenter.Instance.EventTrigger(E_EventType.E_HandCardSelected, index);

            if (theHandCard is HandCard_Entity handCard_Entity
               && handCardDeckVisual.isHide)//创建预览
            {
                handCard_Entity.CreatePreviewEntity();
            }



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

    #region Inputsystem
    void OnMousePosition(InputValue value)
    {
        mousePositionViewport = new Vector2(value.Get<Vector2>().x / Screen.width, value.Get<Vector2>().y / Screen.height);
    }

    public void OnRotateToCreateEntity(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selectedCard is HandCard_Entity handCard_Entity)
            {
                handCard_Entity.RotateToCreateEntity();
            }
        }
    }
    #endregion
}
