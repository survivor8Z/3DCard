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

    public List<HandCardBase> handCards = new List<HandCardBase>();
    public Transform circleCenter;
    public HandCardBase hoveredCard;
    public HandCardBase dragedCard;//���ǿ��Ʊ���ק,������ָʾ
    public HandCardBase selectedCard;
    public int CurrentHandCardCount => handCards.Count;

    [SerializeField]private Transform upMax, downMax;
    [SerializeField]private Vector2 mousePosition;
    [SerializeField]private float minY;
    [SerializeField]private float k = 1;

    private RectTransform theRectTransform;
    

    [SerializeField]private Slots slots;//��λ��
    [SerializeField]private GameObject slotPre;
    [SerializeField]private GameObject cardPre;

    private void Awake()
    {
        theRectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardDraged, OnHandCardDraged);
    }
    private void Update()
    {
        SetDeckPos();

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
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardDraged, OnHandCardDraged);
    }



    //�������λ������������������,ͨ������Slots��ӿ���ʵ�ֻ���
    private void SetDeckPos()
    {
        //����ѡ�е�ʱ��
        if (false)
        {

            return;
        }

        if (mousePosition.y > 0.6) k = 1;
        else if (mousePosition.y > minY) k = Mathf.Lerp(0, 1, (mousePosition.y - minY)/(0.6f - minY));
        else k = 0;
        slots.transform.position = Vector3.Lerp(downMax.position, upMax.position, k);
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

    //�¼���Ӧ
    private void OnHandCardHovered(int index)
    {
        //��������д��
        //hoveredCard = handCards[index-1];

    }
    private void OnHandCardHoveredExit(int index)
    {
        //hoveredCard = null;
    }

    private void OnHandCardDraged(int index)
    {
        
    }

    //���뺯��
    void OnMousePosition(InputValue value)
    {
        mousePosition = new Vector2(value.Get<Vector2>().x / Screen.width, value.Get<Vector2>().y / Screen.height);
    }
}
