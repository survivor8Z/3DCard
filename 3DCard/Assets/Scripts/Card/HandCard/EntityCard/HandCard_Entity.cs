using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_Entity : HandCardBase
{
    public InteractableObject toCreateEntity;
    public string entityPrefabName;
    public bool canCreate = true;
    
    
    /// <summary>
    /// 创建实时预览实体,用于拖拽放置,在Select事件后执行
    /// </summary>
    public void CreatePreviewEntity()
    {
        GameObject obj = PoolMgr.Instance.GetObjSync(entityPrefabName);
        obj.transform.rotation = handCardDeck.player.transform.rotation;
        obj.transform.SetParent(handCardDeck.player.transform);//跟着玩家旋转

        toCreateEntity = obj.GetComponent<InteractableObject>();
    }

    /// <summary>
    /// 顺时针旋转预览实体
    /// </summary>
    public void RotateToCreateEntity()
    {
        if(toCreateEntity == null) return;
        toCreateEntity.transform.rotation *= Quaternion.Euler(0, 90, 0);
    }


    /// <summary>
    /// 创建实时预览实体,用于拖拽放置,update执行
    /// </summary>
    public void TryCreateEntityInGround()
    {
        Vector2Int pivotCoor = MapMgr.Instance.WorldPosToWorldCoor(playerInteract.hitPosition);
        toCreateEntity.transform.position = MapMgr.Instance.WorldCoorToWorldPos(pivotCoor);
        canCreate = true;
        foreach (var relativeCoor in toCreateEntity.obstacleRelativeCoor)
        {
            Vector2Int worldCoor = MapMgr.Instance.RelativeCoorToWorldCoor(
                relativeCoor,
                pivotCoor, 
                MapMgr.Instance.GetDirectionFromRotation(transform.rotation));
            if (MapMgr.Instance.allUnwalkableCoor.Contains(worldCoor))
            {
                canCreate = false;
                break;
            }
        }
        //设置材质 
        if (canCreate)
        {

        }
        else
        {

        }
    }

    public void DelPreViewEntity()
    {
        if (toCreateEntity != null)
        {
            PoolMgr.Instance.PushObj(toCreateEntity.gameObject);
            toCreateEntity = null;
            canCreate = false;
        }
    }

    /// <summary>
    /// 实际打出时执行
    /// </summary>
    public void TryCreateEntity()
    {
        print("TryCreateEntity");
        if (!canCreate ||toCreateEntity==null)
        {
            DelPreViewEntity();
            FailSelectedPlay();
            return;
        }
        
        Vector2Int createInRoomCoorBig = MapMgr.Instance.WorldPosToRoomWorldCoorBig(toCreateEntity.transform.position);
        print(createInRoomCoorBig);
        RoomBase createInRoom = MapMgr.Instance.roomsDict[createInRoomCoorBig];
        createInRoom.AddInteractableObject(toCreateEntity);
        //还要设置材质


        handCardDeck.DelTheCard(index);
    }

}
