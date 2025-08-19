using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_En_Rock : HandCard_Entity,IAddDamage
{
    int IAddDamage.AddDamage
    {
        get
        {
            return 1;
        }
    }

    public override void TryCardDragPlay(IInteractable pointInteractableObject)
    {
        switch(pointInteractableObject)
        {
            case Table://����������ϵ�����
                PlaceCard();
                break;
            case TableCardBase://������ϵ�������
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
        switch(pointInteractableObject)
        {
            default:
                FailSelectedPlay();
                break;
        }

        base.TryCardSelectedPlay(pointInteractableObject);
    }
}
