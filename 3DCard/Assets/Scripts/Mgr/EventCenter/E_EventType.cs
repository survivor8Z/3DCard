using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型 枚举
/// </summary>
public enum E_EventType 
{
    //传入index
    E_HandCardHovered,
    //传入index
    E_HandCardHoveredExit,
    //传入index
    E_HandCardDel,
    //传入index
    E_HandCardLeaveHandCardDeck,
    //传入index
    E_HandCardSelected,
    //传入index
    E_HandCardStartDrag,
    //传入index
    E_HandCardOnDrag,
    //传入index
    E_HandCardEndDrag,
    //传入index
    E_HandCardPointDown,
    //传入index
    E_HandCardPointUp,
    








    //传入HandCardBase
    E_HandCardToTableCard,
    //传入TableCardBase
    E_TableCardToHandCard,











    //传入InteractableSceneObj
    E_PlayerEnterInteractableSceneObjFront,
    //传入InteractableSceneObj
    E_PlayerEnterInteractableSceneObj,
    //传入InteractableSceneObj
    E_PlayerExitInteractableSceneObj,
    //传入InteractableSceneObj
    E_PlayerExitInteractableSceneObjFront
}

