using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HandCardDragLine : HandCardBase
    , IDragHandler
    , IBeginDragHandler
    , IEndDragHandler
{
    public bool isDragged = false;
    public UnityAction onStartDragEvent;//这些事件是分散处理的,不经过EventCenter管理自己
    public UnityAction onDragEvent;
    public UnityAction onEndDragEvent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag: " + index);
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardDraged, index);//经过EventCenter

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag: " + index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag: " + index);
    }
}
