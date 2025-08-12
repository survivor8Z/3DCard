using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_En_Rock : HandCardBase,IAddDamage
{
    

    public override void TryCardDragPlay(IInteractable pointInteractableObject)
    {
        switch(pointInteractableObject)
        {
            case Table:
                PlaceCard();
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
    
    public void PlaceCard()
    {
        Debug.Log("PlaceCard");
        //先是通知手牌库有一个index卡牌离开了,手牌库接着通知HandCardBase离开完了,触发TranslateToTableCard,将手牌转换为桌牌,
        handCardDeck.TheCardLeaveHandDeck(index); 
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

    }

    int IAddDamage.AddDamage
    {
        get
        {
            return 1;
        }
    }
}
