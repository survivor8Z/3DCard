using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_Entity : HandCardBase
{
    //1
    //������ת��Ϊ����
    public void PlaceCard()
    {
        Debug.Log("PlaceCard");
        //����֪ͨ���ƿ���һ��index�����뿪��,���ƿ����֪ͨHandCardBase�뿪����,����TranslateToTableCard,������ת��Ϊ����
        handCardDeck.TheCardLeaveHandDeck(index);
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

        handCardDeck.table.tableCardsControl.tableRootCards.Add(theTableCardBase);
    }

    //2
    //��1������ת��Ϊ���ƻ�����,���õ�������
    public void PlaceToTableCard(TableCardBase theTableCard)
    {
        handCardDeck.TheCardLeaveHandDeck(index);
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

        //���ó�Ϊ������Ƶ��Ӷ���
        this.theTableCardBase.TryStackToTheTableCard(theTableCard);
    }
}
