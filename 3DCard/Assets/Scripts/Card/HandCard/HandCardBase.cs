using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class HandCardBase : CardBase
    , IPointerEnterHandler
    , IPointerExitHandler

    //��Щ����ֱ��������д
    , IDragHandler
    , IBeginDragHandler
    , IEndDragHandler
    , IPointerUpHandler
    , IPointerDownHandler
{
    

    public HandCardDeck handCardDeck;
    public RectTransform slotRectTrans;
    public Transform selectedToPos=>handCardDeck.handCardDeckVisual.handCardSelectedToPos;
    public int index;
    public bool isHovered = false;
    public bool isSelected = false; 
    public bool isDragging = false;


    //��TableCardת�����
    public TableCardBase theTableCardBase; //�����ʵ�忨��,���ж�Ӧ��TableCardBase..


    #region �������ں���
    private void Awake()
    {
        RegisterDragFunc();


        theTableCardBase = GetComponent<TableCardBase>();
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardDel, Deleted);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardLeaveHandCardDeck, TranslateToTableCard);
    }


    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardDel, Deleted);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardLeaveHandCardDeck, TranslateToTableCard);
    }
    #endregion


    /// <summary>
    /// ��HandCardDeck����
    /// </summary>
    public void Deleted(int index)
    {
        if (this.index != index) return;
        Destroy(gameObject);//TODO:�����֮����˵
    }

    public void TranslateToTableCard(int index)
    {
        if (this.index != index) return;
        //TODO:���˿������ó�TableCard
        

        this.enabled = false;

        theTableCardBase.enabled = true;
    }


    public void ResetState()
    {
        isDragging = false;
        isSelected = false;
        ExecuteEvents.ExecuteHierarchy<IEndDragHandler>(
            gameObject,
            new PointerEventData(EventSystem.current),
            ExecuteEvents.endDragHandler
        );
    }

    #region ʵ�ʿ��ƴ��Ч�����
    #region DragedPlay
    [ShowInInspector]
    private Dictionary<CardDragedPlayID, int> _dragedFuncDic = new();
    public IReadOnlyDictionary<CardDragedPlayID, int> DragedFuncDic
    {
        get
        {
            _dragedFuncDic ??= new Dictionary<CardDragedPlayID, int>();
            return _dragedFuncDic;
        }
        protected set
        {
            _dragedFuncDic = value != null
                ? new Dictionary<CardDragedPlayID, int>(value)
                : new Dictionary<CardDragedPlayID, int>();
        }
    }
    protected void AddDragedFuncDic(CardDragedPlayID dragedPlayID,int funcID)
    {
        if(_dragedFuncDic.ContainsKey(dragedPlayID))
        {
            Debug.LogWarning($"DragedFuncDic already contains {dragedPlayID}, updating value.");
            _dragedFuncDic[dragedPlayID] = funcID;
            return;
        }
        _dragedFuncDic[dragedPlayID] = funcID;
    }

    protected void RemoveDragedFuncDic(CardDragedPlayID dragedPlayID)
    {
        if (_dragedFuncDic.ContainsKey(dragedPlayID))
        {
            _dragedFuncDic.Remove(dragedPlayID);
        }
        else
        {
            Debug.LogWarning($"DragedFuncDic does not contain {dragedPlayID}, cannot remove.");
        }
    }
    private bool CanDragPlayCard(CardDragedPlayID cardDragedPlayID)
    {
        if (DragedFuncDic.ContainsKey(cardDragedPlayID))
            return true;
        return false;
    }

    /// <summary>
    /// ��PlayerInteract����
    /// </summary>
    public void TryCardDragPlay(CardDragedPlayID cardDragedPlayID)
    {
        Debug.Log("TryCardDragPlay");
        if (CanDragPlayCard(cardDragedPlayID))
        {
            TrulyDragPlay(DragedFuncDic[cardDragedPlayID]);
        }
        else
        {
            //ִ�в��ܴ��(��Ч��Ч�ȵ�)
        }
        handCardDeck.dragedCard = null; // �����ק״̬
    }

    //��������ק���Ч��,������ʵ��
    protected virtual void TrulyDragPlay(int funcID)
    {

    }
    //������_dragedFuncDic��ʼ�����
    [SerializeField]private List<CardDragedPlayID> cardDragedPlayIDs = new List<CardDragedPlayID>();
    [SerializeField]private List<int> funcIDs = new List<int>();
    private void RegisterDragFunc()
    {
        for(int i = 0; i < cardDragedPlayIDs.Count; i++)
        {
            Debug.Log($"Registering DragedFuncDic: {cardDragedPlayIDs[i]} with funcID {funcIDs[i]}");
            AddDragedFuncDic(cardDragedPlayIDs[i], funcIDs[i]);
        }
    }
    #endregion
    
    
    #region SelectedPlay
    [ShowInInspector]
    private Dictionary<CardSelectedPlayID, int> _selectedFuncDic=new();
    public IReadOnlyDictionary<CardSelectedPlayID, int> SelectedFuncDic
    {
        get
        {
            _selectedFuncDic ??= new Dictionary<CardSelectedPlayID, int>();
            return _selectedFuncDic;
        }
        protected set
        {
            _selectedFuncDic = value != null
                ? new Dictionary<CardSelectedPlayID, int>(value)
                : new Dictionary<CardSelectedPlayID, int>();
        }
    }
    protected void AddSelectedFuncDic(CardSelectedPlayID selectedPlayID, int funcID)
    {
        if (_selectedFuncDic.ContainsKey(selectedPlayID))
        {
            Debug.LogWarning($"SelectedFuncDic already contains {selectedPlayID}, updating value.");
            _selectedFuncDic[selectedPlayID] = funcID;
            return;
        }
        _selectedFuncDic[selectedPlayID] = funcID;
    }
    protected void RemoveSelectedFuncDic(CardSelectedPlayID selectedPlayID)
    {
        if (_selectedFuncDic.ContainsKey(selectedPlayID))
        {
            _selectedFuncDic.Remove(selectedPlayID);
        }
        else
        {
            Debug.LogWarning($"SelectedFuncDic does not contain {selectedPlayID}, cannot remove.");
        }
    }

    private bool CanSelectedPlayCard(CardSelectedPlayID cardSelectedPlayID)
    {
        if(SelectedFuncDic.ContainsKey(cardSelectedPlayID))
            return true;
        return false;
    }
    

    /// <summary>
    /// ��PlayerInteract����
    /// </summary>
    public void TryCardSelectedPlay(CardSelectedPlayID cardSelectedPlayID)
    {
        Debug.Log("TryCardSelectedPlay");
        if (CanSelectedPlayCard(cardSelectedPlayID))
        {
            TrulySelectedPlay(SelectedFuncDic[cardSelectedPlayID]);
        }
        else
        {
            //ִ�в��ܴ��(��Ч��Ч�ȵ�)
        }
        
        handCardDeck.selectedCard = null;
        isSelected = false;
    }
    //������ѡ�д��Ч��,������ʵ��
    protected virtual void TrulySelectedPlay(int funcID)
    {

    }
    #endregion
    #endregion

    //�¼���Ӧ


    #region ugui����ӿ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (isSelected) return;
        if (isDragging) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardHovered, index);
        isHovered = true;
        handCardDeck.hoveredCard = this; // ���õ�ǰ��ͣ������
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (isDragging) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardHoveredExit, index);
        isHovered = false;
        if (handCardDeck.hoveredCard == this) // �����ǰ��ͣ�����������
        {
            handCardDeck.hoveredCard = null; // �����ͣ״̬
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSelected) return;
        if(cardSO.cardType == E_CardType.E_Entity)
        {
            if(handCardDeck.player.playerMove.inSceneObj is Table)
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_HandCardStartDrag, index);
                handCardDeck.dragedCard = this; // ���õ�ǰ��ק������
                isDragging = true;
            }
        }
        else if(cardSO.cardType == E_CardType.E_Modificatory)
        {
            return;
        }
        else if(cardSO.cardType == E_CardType.E_Behavior)
        {
            
            handCardDeck.dragedCard = this; // ���õ�ǰ��ק������
            isDragging = true;
        }
        else if(cardSO.cardType == E_CardType.E_Condition)
        {
            return;
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSelected) return;
        //EventCenter.Instance.EventTrigger(E_EventType.E_HandCardOnDrag, index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSelected) return;
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardEndDrag, index);
        isDragging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardSO.cardType == E_CardType.E_Entity)
        {

        }
        else if (cardSO.cardType == E_CardType.E_Modificatory)
        {

        }
        else if (cardSO.cardType == E_CardType.E_Behavior)
        {
            return;
        }
        else if (cardSO.cardType == E_CardType.E_Condition)
        {
            
        }
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardPointDown, index);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging) return;
        if (!isHovered) return;
        if (cardSO.cardType == E_CardType.E_Entity)
        {

        }
        else if (cardSO.cardType == E_CardType.E_Modificatory)
        {

        }
        else if (cardSO.cardType == E_CardType.E_Behavior)
        {
            return;
        }
        else if (cardSO.cardType == E_CardType.E_Condition)
        {

        }
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardPointUp, index);
    }
    #endregion
}
