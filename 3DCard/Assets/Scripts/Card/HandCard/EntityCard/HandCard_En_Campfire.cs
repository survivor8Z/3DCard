using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_En_Campfire : HandCard_Entity
{
	public override void TryCardDragPlay(IInteractable pointInteractableObject)
    {
        switch (pointInteractableObject)
        {
            case Table://如果是桌子拖到桌面
                PlaceCard();
                break;
            case TableCardBase://如果是拖到桌牌上
                PlaceToTableCard(pointInteractableObject as TableCardBase);
                break;
            default:
                FailDragPlay();
                break;
        }
        base.TryCardDragPlay(pointInteractableObject);
    }

    public override void TryCardSelectedPlay(IInteractable pointInteractableObject)
    {
        base.TryCardSelectedPlay(pointInteractableObject);
    }
}
