using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStart : RoomBase
{
    public override void SetWall()
    {
        //�������ܷ���
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(transform.forward)))
        {
            AddWall("DoorWall", Vector2Int.up * 5, (wallBase) => AddTheObstacle(wallBase));
        }
        else
        {
            List<Vector2Int> randomRelativeCoor = new List<Vector2Int> { Vector2Int.right * 5, Vector2Int.down * 5, Vector2Int.left * 5 };
            int randomIndex = Random.Range(0, randomRelativeCoor.Count);
            WallToDoorWall(randomRelativeCoor[randomIndex]);
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(transform.right)))
        {
            AddWall("NormalWall", Vector2Int.right * 5, (wallBase) => AddTheObstacle(wallBase));
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(-transform.forward)))
        {
            AddWall("NormalWall", Vector2Int.down * 5, (wallBase) => AddTheObstacle(wallBase));
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(-transform.right)))
        {
            AddWall("NormalWall", Vector2Int.left * 5, (wallBase) => AddTheObstacle(wallBase));
        }



        ////��Ϊ���첽���Բ���
        //if (!wallDict.ContainsKey(Vector2Int.up * 5))
        //{
        //    print("û��ǰǽ,�����ǽ");
           
        //}



    }
    
}
