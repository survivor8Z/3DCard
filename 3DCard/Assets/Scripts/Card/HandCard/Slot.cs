using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int index;
    private LayoutElement layoutElement;
    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardDel, Deleted);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardLeaveHandCardDeck, Deleted);
    }

    //TODO:对象池之后再说
    public void Deleted(int index)
    {
        if (this.index != index) return;
        Destroy(gameObject);
    }
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardDel, Deleted);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardLeaveHandCardDeck, Deleted);
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
