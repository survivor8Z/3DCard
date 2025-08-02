using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
/// <summary>
/// �������ƹ���:��ɾ����,��������λ��
/// </summary>

public class HandCardDeck : MonoBehaviour
{
    public InteractableObject currentPointInteractableObject;//��ǰָ��Ŀɽ�������

    public List<HandCardBase> handCards = new List<HandCardBase>();
    public Transform circleCenter;
    public HandCardBase hoveredCard;
    public HandCardBase dragedCard;//���ǿ��Ʊ���ק,������ָʾ
    public HandCardBase selectedCard;
    public int CurrentHandCardCount => handCards.Count;

    public Vector2 mousePositionViewport;
   

    private RectTransform theRectTransform;

    //д��HandCardDeckVisual����
    //public Transform handCardSelectedToPos;
    public HandCardDeckVisual handCardDeckVisual;
    
    public Slots slots;//��λ��
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
    public void AddCard()//����
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
        DelTheCard(selectedCard.index); // ɾ��ѡ�еĿ���
        DelTheCard(dragedCard.index); // ɾ����ק�Ŀ���
    }

    //�¼���Ӧ
    public void OnHandCardClick(int index)
    {
        if (index <= 0 || index > handCards.Count) return;
        HandCardBase theHandCard = handCards[index - 1];
        if (theHandCard == selectedCard&&theHandCard.isSelected)
        {
            
            theHandCard.isSelected = false; // ���ѡ��״̬
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

            //TODO:����slot
            Slot theSlot = slots.slotsList[index - 1];
            slots.slotsList.RemoveAt(index - 1);
            slots.slotsList.Add(theSlot);

            handCards.RemoveAt(index - 1);
            handCards.Add(theHandCard);
            
            ResetCardIndex();
            slots.ResetSlots();

        }


    }


    


    //���뺯��
    void OnMousePosition(InputValue value)
    {
        mousePositionViewport = new Vector2(value.Get<Vector2>().x / Screen.width, value.Get<Vector2>().y / Screen.height);
    }
}
