using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_Entity : HandCardBase
{
    //1
    //将手牌转换为桌牌
    public void PlaceCard()
    {
        Debug.Log("PlaceCard");
        //先是通知手牌库有一个index卡牌离开了,手牌库接着通知HandCardBase离开完了,触发TranslateToTableCard,将手牌转换为桌牌
        handCardDeck.TheCardLeaveHandDeck(index);
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

        handCardDeck.table.tableCardsControl.tableRootCards.Add(theTableCardBase);
    }

    //2
    //在1将手牌转换为桌牌基础上,放置到桌牌上
    public void PlaceToTableCard(TableCardBase theTableCard)
    {
        handCardDeck.TheCardLeaveHandDeck(index);
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

        //设置成为这个桌牌的子对象
        this.theTableCardBase.TryStackToTheTableCard(theTableCard);
    }
}
