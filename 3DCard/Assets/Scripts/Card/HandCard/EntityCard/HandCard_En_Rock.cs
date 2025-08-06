using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_En_Rock : HandCardBase
{
    

    protected override void TrulyDragPlay(int funcID)
    {
        switch(funcID)
        {
            case 1:
                PlaceCard();
                break;
            default:
                Debug.LogError("RockCard funcID not found: " + funcID);
                break;
        }
    }

    protected override void TrulySelectedPlay(int funcID)
    {
        
    }

    // funcID 1 放置卡牌
    public void PlaceCard()
    {
        Debug.Log("PlaceCard");
        //先是通知手牌库有一个index卡牌离开了,手牌库接着通知HandCardBase离开完了,触发TranslateToTableCard,将手牌转换为桌牌,
        handCardDeck.TheCardLeaveHandDeck(index); 
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

    }

}
