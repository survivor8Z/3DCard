using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public struct CardCombinationPlay
//{
//    public int dragedCardID;
//    public int selectedCardID;
//    public int pointedInteractableObjID;
//    public CardCombinationPlay(int dragedCardID,int selectedCardID,int pointedInteractableObjID )
//    {
//        this.dragedCardID = dragedCardID;
//        this.selectedCardID = selectedCardID;
//        this.pointedInteractableObjID = pointedInteractableObjID;
//    }
    
//}

//public struct CardDragedPlay
//{
//    public int dragedCardID;
//    public int pointedInteractableObjID;
//    public CardDragedPlay(int dragedCardID, int pointedInteractableObjID)
//    {
//        this.dragedCardID = dragedCardID;
//        this.pointedInteractableObjID = pointedInteractableObjID;
//    }
//}
//public struct CardSelectedPlay
//{
//    public int selectedCardID;
//    public int pointedInteractableObjID;
//    public CardSelectedPlay(int selectedCardID, int pointedInteractableObjID)
//    {
//        this.selectedCardID = selectedCardID;
//        this.pointedInteractableObjID = pointedInteractableObjID;
//    }
//}

//下面是直接包裹,不用id
public struct CardCombinationPlay
{
    public HandCardBase dragedCard;
    public HandCardBase selectedCard;
    public IInteractable pointedInteractableObj;

    public CardCombinationPlay(HandCardBase dragedCard, HandCardBase selectedCard, IInteractable pointedInteractableObj)
    {
        this.dragedCard = dragedCard;
        this.selectedCard = selectedCard;
        this.pointedInteractableObj = pointedInteractableObj;
    }
}

public struct CardDragedPlay
{
    public HandCardBase dragedCard;
    public IInteractable pointedInteractableObj;
    public CardDragedPlay(HandCardBase dragedCard, IInteractable pointedInteractableObj)
    {
        this.dragedCard = dragedCard;
        this.pointedInteractableObj = pointedInteractableObj;
    }
}

public struct CardSelectedPlay
{
    public HandCardBase selectedCard;
    public IInteractable pointedInteractableObj;
    public CardSelectedPlay(HandCardBase selectedCard, IInteractable pointedInteractableObj)
    {
        this.selectedCard = selectedCard;
        this.pointedInteractableObj = pointedInteractableObj;
    }
}
