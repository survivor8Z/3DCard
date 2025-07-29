using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HandCardPlaceable : HandCardBase
    , IPointerUpHandler
    , IPointerDownHandler
    , IBeginDragHandler
{
    public bool isSelected = false; // 是否被选中
    public UnityAction onClickedUpEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        

    }
    public void OnBeginDrag(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_HandCardSelected, index);
        isSelected = true;
    }
}
