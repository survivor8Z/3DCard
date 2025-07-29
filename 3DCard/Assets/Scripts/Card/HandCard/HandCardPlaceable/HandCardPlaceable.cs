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
    public bool isSelected = false; // �Ƿ�ѡ��
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
