using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour 
{
    public int index;
    private LayoutElement layoutElement;
    [SerializeField]private RectTransform theRectTransform;

    //for test
    [SerializeField]private Vector3 EulerAngle;
    [SerializeField]private Vector2 ScreenPos;
    public Vector2 mouseScreenPos;
    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        theRectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardDel, Deleted);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardLeaveHandCardDeck, Deleted);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardSlotDel, Deleted);
    }
    

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardDel, Deleted);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardLeaveHandCardDeck, Deleted);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardSlotDel, Deleted);
    }


    //TODO:对象池之后再说
    public void Deleted(int index)
    {
        if (this.index != index) return;
        Destroy(gameObject);
    }
    //事件响应
    private void OnHandCardHovered(int index)
    {
        if(this.index != index) return;
        layoutElement.flexibleWidth = 2f;
    }
    private void OnHandCardHoveredExit(int index)
    {
        if (this.index != index) return;
        layoutElement.flexibleWidth = 1f;
    }

    
}
