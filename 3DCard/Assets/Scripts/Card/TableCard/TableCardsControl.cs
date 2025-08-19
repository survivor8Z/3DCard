using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �����������Ƶ�
/// </summary>
public class TableCardsControl : MonoBehaviour
{
    public List<TableCardBase> tableRootCards = new List<TableCardBase>();
    public TableCardBase currentDragCard;

    public HandCardDeck handCardDeck=>currentDragCard.thehandCardBase.handCardDeck;

    //�Ż����
    public float toHandY;

    #region �������ں���
    private void Awake()
    {
    }
    private void Update()
    {
        //print(AboutToCreateSlotIndex(Input.mousePosition));
    }


    #endregion

    

    /// <summary>
    /// ����ȷ�Ͻ�Ҫ�����Ʒŵ������е��ĸ���λ,index��listindex
    /// 0<=res<=handCardDeck.slots.slotsList.Count
    /// </summary>
    /// <param name="MouseScreenPosition"></param>
    /// <returns></returns>
    public int AboutToCreateSlotIndex(Vector2 MouseScreenPosition)
    {
        int res = 0;
        while (res < handCardDeck.slots.slotsList.Count
               && Camera.main.WorldToScreenPoint(handCardDeck.slots.slotsList[res].transform.position).x < MouseScreenPosition.x)
        {
            res++;
        }
        return res;
    }
    /// <summary>
    /// ������Ѿ�������һ��slot��
    /// ����ȷ�Ͻ�Ҫ�����Ʒŵ������е��ĸ���λ,index��listindex
    /// 0<=res<handCardDeck.slots.slotsList.Count
    /// </summary>
    /// <param name="MouseScreenPosition"></param>
    /// <returns></returns>
    public int AboutToSetSlotIndex(Vector2 MouseScreenPosition)
    {
        int res = 0;
        while(res < handCardDeck.slots.slotsList.Count-1
               && Camera.main.WorldToScreenPoint(handCardDeck.slots.slotsList[res].transform.position).x < MouseScreenPosition.x)
        {
            res++;
        }
        return res;
    }


    Slot currentToSlot;
    /// <summary>
    /// ���Բ���slot,
    /// </summary>
    public void TryInsertSlot(Vector2 MouseScreenPosition)
    {
        if (handCardDeck.maxCount <= handCardDeck.handCards.Count
            || MouseScreenPosition.y / Screen.height >= toHandY)
        {
            //ִ�зŻ�ʧ���߼�,����еĻ�,��������ק�����˻�Ҫɾ��
            if(currentToSlot != null)
            {
                handCardDeck.RemoveTheSlot(currentToSlot);
                currentToSlot = null;
            }
            currentDragCard.toSlot = null;

            return;
        }
        
        if (currentToSlot == null)
        {
            //���index��list��index
            int insertSlotListIndex = AboutToCreateSlotIndex(MouseScreenPosition);
            //print("insertSlotListIndex" + insertSlotListIndex);
            currentToSlot = handCardDeck.InsertASlot(insertSlotListIndex);

        }
        else
        {
            int setSlotListIndex = AboutToSetSlotIndex(MouseScreenPosition);
            //print("setSlotListIndex " + setSlotListIndex);
            if (setSlotListIndex != currentToSlot.index-1)
            {
                handCardDeck.SetTheSlotPos(currentToSlot, setSlotListIndex);
            }
        }
        currentDragCard.toSlot = currentToSlot;


    }

    // �¼���Ӧ
    public void OnTheTableCardDrag(TableCardBase theTableCard)
    {
        currentDragCard = theTableCard;
        TryInsertSlot(Input.mousePosition);
    }

    public void OnTheTableCardEndDrag(TableCardBase theTableCard)
    {
        //���������slot,�������ֱ��ִ��ת����

        if(currentDragCard != null&&currentToSlot!=null)
        {
            //��ʼ��ʽת�����Ƶ�����
            print(theTableCard.name + " translate to hand card");
            currentDragCard.TranslateToHandCard(currentToSlot);
        }
        else if(currentDragCard!=null && currentToSlot == null)
        {
            
            
        }
        




        currentToSlot = null; //��յ�ǰ��slot
        currentDragCard = null; 
    }

    
}
