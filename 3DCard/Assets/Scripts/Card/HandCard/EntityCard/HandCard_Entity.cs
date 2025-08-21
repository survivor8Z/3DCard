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
    public void CreatePreviewEntity(string entityPrefabName)
    {
        GameObject obj = PoolMgr.Instance.GetObjSync(entityPrefabName);
        obj.transform.SetParent(MapMgr.Instance.currentRoom.transform);
        obj.transform.rotation = handCardDeck.player.transform.rotation;
        obj.transform.SetParent(handCardDeck.player.transform);//跟着玩家旋转

        toCreateEntity = obj.GetComponent<InteractableObject>();
    }

    /// <summary>
    /// 创建实时预览实体,用于拖拽放置,update执行
    /// </summary>
    public void TryCreateEntityInGround(string entityPrefabName)
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


    public void CreateEntity(Vector2Int pivotFront)
    {

    }

}
