using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
/// <summary>
/// 负责手牌管理:增删手牌,设置整体位置
/// </summary>

public class HandCardDeck : MonoBehaviour
{
    public InteractableObject currentPointInteractableObject;//当前指向的可交互物体

    public List<HandCardBase> handCards = new List<HandCardBase>();
    public Transform circleCenter;
    public HandCardBase hoveredCard;
    public HandCardBase dragedCard;//不是卡牌被拖拽,会有线指示
    public HandCardBase selectedCard;
    public int CurrentHandCardCount => handCards.Count;

    public Vector2 mousePositionViewport;
   

    private RectTransform theRectTransform;

    //写到HandCardDeckVisual里了
    //public Transform handCardSelectedToPos;
    public HandCardDeckVisual handCardDeckVisual;
    
    public Slots slots;//槽位盘
    [SerializeField]private GameObject slotPre;
    [SerializeField]private GameObject cardPre;

    private void Awake()
    {
        theRectTransform = GetComponent<RectTransform>();
        handCardDeckVisual = GetComponent<HandCardDeckVisual>();
    }
    private void Start()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardPointUp, OnHandCardClick);
    }
    private void Update()
    {
        
        //test
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddCard();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            DelTheCard(1);
        }

    }
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardPointUp, OnHandCardClick);
    }



    

    [ContextMenu("Add Card")]
    public void AddCard()//测试
    {
        GameObject slotObj = Instantiate(slotPre, slots.transform);
        slotObj.name = "Slot" + (slots.transform.childCount);
        GameObject card = Instantiate(cardPre, transform);
        card.name = "Card" + (handCards.Count + 1);
        
        HandCardBase theHandCard = card.GetComponent<HandCardBase>();
        Slot theSlot = slotObj.GetComponent<Slot>();
        
        handCards.Add(theHandCard);

        slots.slotsList.Add(theSlot);
        theSlot.index = handCards.Count;

        theHandCard.handCardDeck = this;
        theHandCard.slotRectTrans = slotObj.GetComponent<RectTransform>();
        theHandCard.index = handCards.Count;

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
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardDel, index);
        ResetCardIndex();
    }

    public void ResetCardIndex()
    {
        for(int i = 0; i < handCards.Count; i++)
        {
            handCards[i].index = i + 1;
            slots.slotsList[i].index = i + 1;
        }
    }

    public void CardCombinationPlay()
    {
        Debug.Log("CardCombinationPlay");
        DelTheCard(selectedCard.index); // 删除选中的卡牌
        DelTheCard(dragedCard.index); // 删除拖拽的卡牌
    }

    //事件响应
    public void OnHandCardClick(int index)
    {
        if (index <= 0 || index > handCards.Count) return;
        HandCardBase theHandCard = handCards[index - 1];
        if (theHandCard == selectedCard&&theHandCard.isSelected)
        {
            
            theHandCard.isSelected = false; // 清除选中状态
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

            //TODO:更新slot
            Slot theSlot = slots.slotsList[index - 1];
            slots.slotsList.RemoveAt(index - 1);
            slots.slotsList.Add(theSlot);

            handCards.RemoveAt(index - 1);
            handCards.Add(theHandCard);
            
            ResetCardIndex();
            slots.ResetSlots();

        }


    }


    


    //输入函数
    void OnMousePosition(InputValue value)
    {
        mousePositionViewport = new Vector2(value.Get<Vector2>().x / Screen.width, value.Get<Vector2>().y / Screen.height);
    }
}
