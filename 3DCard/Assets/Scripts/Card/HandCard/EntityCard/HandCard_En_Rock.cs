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

    // funcID 1 ���ÿ���
    public void PlaceCard()
    {
        Debug.Log("PlaceCard");
        //����֪ͨ���ƿ���һ��index�����뿪��,���ƿ����֪ͨHandCardBase�뿪����,����TranslateToTableCard,������ת��Ϊ����,
        handCardDeck.TheCardLeaveHandDeck(index); 
        EventCenter.Instance.EventTrigger<HandCardBase>(E_EventType.E_HandCardToTableCard, this);

    }

}
