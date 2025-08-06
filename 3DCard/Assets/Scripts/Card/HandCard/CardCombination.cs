using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CardCombinationPlayID
{
    public int dragedCardID;
    public int selectedCardID;
    public int pointedInteractableObjID;
    public CardCombinationPlayID(int dragedCardID,int selectedCardID,int pointedInteractableObjID )
    {
        this.dragedCardID = dragedCardID;
        this.selectedCardID = selectedCardID;
        this.pointedInteractableObjID = pointedInteractableObjID;
    }
    
}

public struct CardDragedPlayID
{
    public int dragedCardID;
    public int pointedInteractableObjID;
    public CardDragedPlayID(int dragedCardID, int pointedInteractableObjID)
    {
        this.dragedCardID = dragedCardID;
        this.pointedInteractableObjID = pointedInteractableObjID;
    }
}
public struct CardSelectedPlayID
{
    public int selectedCardID;
    public int pointedInteractableObjID;
    public CardSelectedPlayID(int selectedCardID, int pointedInteractableObjID)
    {
        this.selectedCardID = selectedCardID;
        this.pointedInteractableObjID = pointedInteractableObjID;
    }
}
