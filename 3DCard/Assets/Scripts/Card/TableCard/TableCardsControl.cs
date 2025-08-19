using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用来管理桌牌的
/// </summary>
public class TableCardsControl : MonoBehaviour
{
    public List<TableCardBase> tableRootCards = new List<TableCardBase>();
    public TableCardBase currentDragCard;

    public HandCardDeck handCardDeck=>currentDragCard.thehandCardBase.handCardDeck;

    //放回相关
    public float toHandY;

    #region 生命周期函数
    private void Awake()
    {
    }
    private void Update()
    {
        //print(AboutToCreateSlotIndex(Input.mousePosition));
    }


    #endregion

    

    /// <summary>
    /// 用于确认将要把桌牌放到手牌中的哪个槽位,index是listindex
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
    /// 这个是已经创建了一个slot了
    /// 用于确认将要把桌牌放到手牌中的哪个槽位,index是listindex
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
    /// 尝试插入slot,
    /// </summary>
    public void TryInsertSlot(Vector2 MouseScreenPosition)
    {
        if (handCardDeck.maxCount <= handCardDeck.handCards.Count
            || MouseScreenPosition.y / Screen.height >= toHandY)
        {
            //执行放回失败逻辑,如果有的话,如果这次拖拽插入了还要删除
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
            //这个index是list的index
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

    // 事件响应
    public void OnTheTableCardDrag(TableCardBase theTableCard)
    {
        currentDragCard = theTableCard;
        TryInsertSlot(Input.mousePosition);
    }

    public void OnTheTableCardEndDrag(TableCardBase theTableCard)
    {
        //如果插入了slot,这个就是直接执行转换了

        if(currentDragCard != null&&currentToSlot!=null)
        {
            //开始正式转换桌牌到手牌
            print(theTableCard.name + " translate to hand card");
            currentDragCard.TranslateToHandCard(currentToSlot);
        }
        else if(currentDragCard!=null && currentToSlot == null)
        {
            
            
        }
        




        currentToSlot = null; //清空当前的slot
        currentDragCard = null; 
    }

    
}
