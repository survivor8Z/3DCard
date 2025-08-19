using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
/// <summary>
/// �������ƹ���:��ɾ����,��������λ��
/// </summary>

public class HandCardDeck : SerializedMonoBehaviour
{
    public Player player;
    public InteractableObject currentPointInteractableObject;//��ǰָ��Ŀɽ�������

    public int maxCount = 10;
    public List<HandCardBase> handCards = new List<HandCardBase>();
    public Transform circleCenter;
    public HandCardBase hoveredCard;
    public HandCardBase dragedCard;//���ǿ��Ʊ���ק,������ָʾ
    public HandCardBase selectedCard;
    public int CurrentHandCardCount => handCards.Count;

    public Vector2 mousePositionViewport;
   

    private RectTransform theRectTransform;

    public HandCardDeckVisual handCardDeckVisual;
    
    public Slots slots;//��λ��
    [SerializeField]private GameObject slotPre;

    //����ȡ��
    public bool isCancel = false;

    //����������ת��
    public Table table => MapMgr.Instance.currentRoom.table;

    //���ڴ���Ԥ��λ
    public int currentIndex;


    #region �������ں���
    private void Awake()
    {

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

    #region ʵ�ʿ��ƴ��Ч�����
    /// <summary>
    /// ��PlayerInteract����
    /// </summary>
    public void TryCardCombinationPlay(IInteractable pointInteractableObject)
    {
        switch (dragedCard)
        {
            case IAttack attack:
                if(pointInteractableObject is IDamageable damageable)
                {
                    if (selectedCard is IAddDamage addDamage)
                    {
                        attack.AttackWithAddDamage(damageable, addDamage);
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


        selectedCard.isSelected = false; // ���ѡ��״̬
        selectedCard = null;
        dragedCard = null;
    }






    public void FailCombinationPlay()
    {
        //UNDONE
        print("FailCombinationPlay");
    }
    #endregion


    /// <summary>
    /// ���һ������,һ������interactableObject�������
    /// </summary>
    /// <param name="cardSO"></param>
    public void AddCard(CardSO cardSO)
    {
        if (handCards.Count >= maxCount)
        {
            Debug.Log("HandCardDeck: AddCard: Hand card count exceeds max limit");
            return;
        }
        //GameObject slotObj = Instantiate(slotPre, slots.transform);
        GameObject slotObj = PoolMgr.Instance.GetObjSync("Slot");
        slotObj.transform.SetParent(slots.transform);
        slotObj.transform.localPosition = Vector3.zero;
        slotObj.transform.localScale = Vector3.one;

        //GameObject card = Instantiate(cardPreDic[cardSO], transform);
        GameObject card = PoolMgr.Instance.GetObjSync("HandCard_" + cardSO.cardEnglishName);

        card.transform.position = slots.transform.position;
        card.transform.SetParent(transform);
        card.transform.localScale = Vector3.one;
        //card.name = "Card" + (handCards.Count + 1);

        HandCardBase theHandCard = card.GetComponent<HandCardBase>();
        Slot theSlot = slotObj.GetComponent<Slot>();

        handCards.Add(theHandCard);

        slots.slotsList.Add(theSlot);
        theSlot.index = handCards.Count;

        theHandCard.cardSO = cardSO; // ���ÿ�������
        theHandCard.handCardDeck = this;
        theHandCard.slotRectTrans = slotObj.GetComponent<RectTransform>();
        theHandCard.index = handCards.Count;

    }
    

    /// <summary>
    /// ����һ��Slot,����������
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
    /// �Ƴ�һ��Slot,���slot��û�п��Ƶ�,����п���,�������������
    /// </summary>
    /// <param name="slot"></param>
    public void RemoveTheSlot(Slot slot)
    {
        slots.slotsList.RemoveAt(slot.index - 1);
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardSlotDel, slot.index);
        ResetSlotsIndex();
    }

    /// <summary>
    /// index:���Ƶ�����,��1��ʼ
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
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardDel, index);//���ֻ��֪ͨHandCardBaseɾ��(��Ȼ˵�Ҿ��ú�����)
        ResetCardIndex();
    }

    /// <summary>
    /// index:���Ƶ�����,��1��ʼ
    /// ��AddCard���,���ǲ��Ƴ�
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
            slots.slotsList[i].layoutElement.flexibleWidth = 1f;
        }   
    }

    public void ResetCardIndex()
    {
        for(int i = 0; i < handCards.Count; i++)
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
            handCards[i].transform.SetSiblingIndex(i+1);
            slots.slotsList[i].transform.SetSiblingIndex(i);
            slots.slotsList[i].layoutElement.flexibleWidth = 1f;
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


    #region �¼���Ӧ

    public void OnHandCardClick(int index)
    {
        if(isCancel)return;
        if (index <= 0 || index > handCards.Count) return;
        HandCardBase theHandCard = handCards[index - 1];
        if (theHandCard == selectedCard&&theHandCard.isSelected)
        {
            
            theHandCard.isSelected = false; // ���ѡ��״̬+

            selectedCard.transform.SetParent(transform);
            selectedCard = null; // ���ѡ������
        }
        else
        {
            if (selectedCard != null)
            {
                selectedCard.isSelected = false;
            }

            theHandCard.isSelected = true; // ����Ϊѡ��״̬
            selectedCard = theHandCard; // ����ѡ������

            
            EventCenter.Instance.EventTrigger(E_EventType.E_HandCardSelected, index);
            
            
            

            //����slot
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


    //���뺯��
    void OnMousePosition(InputValue value)
    {
        mousePositionViewport = new Vector2(value.Get<Vector2>().x / Screen.width, value.Get<Vector2>().y / Screen.height);
    }
}
