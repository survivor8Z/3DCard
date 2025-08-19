using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : SingletonMono<MapMgr>
{
    public RoomBase currentRoom;//在Player那里赋值了
    [ShowInInspector]public Dictionary<Vector2Int, RoomBase> roomsDict=new();//是大坐标,以RoomSize为单位的坐标系
    public readonly int RoomSize = 10;//房间坐标间隔
    


    [ShowInInspector]public HashSet<Vector2Int> allUnwalkableCoor = new();
    public readonly int cellSize = 1;


    #region 坐标转换
    /// <summary>
    /// 大坐标
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public Vector2Int WorldPosToRoomWorldCoorBig(Vector3 worldPos)
    {
        // 将世界坐标除以房间大小，然后四舍五入
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / RoomSize),
            Mathf.RoundToInt(worldPos.z / RoomSize)
        );
    }

    /// <summary>
    /// 大坐标
    /// </summary>
    /// <param name="roomWorldCoor"></param>
    /// <returns></returns>
    public Vector3 RoomWorldCoorToWorldPosBig(Vector2Int roomWorldCoor)
    {
        // 将房间网格坐标乘以房间大小
        return new Vector3(
            roomWorldCoor.x * RoomSize,
            0,
            roomWorldCoor.y * RoomSize
        );
    }

    public Vector2Int WorldPosToWorldCoor(Vector3 worldPos)
    {
        // 将世界坐标除以单元格大小，然后四舍五入
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.z / cellSize)
        );
    }

    public Vector3 WorldCoorToWorldPos(Vector2Int worldCoor)
    {
        // 将网格坐标乘以单元格大小
        return new Vector3(
            worldCoor.x * cellSize,
            0,
            worldCoor.y * cellSize
        );
    }

    public Vector2Int WorldCoorToRelativeCoor(Vector2Int worldCoor,Vector2Int worldPivotCoor,Vector2Int pivotFront)
    {
        // 1. 平移：计算相对于原点的世界坐标
        Vector2Int relativeCoor = worldCoor - worldPivotCoor;
        // 2. 旋转：根据相对坐标系的朝向进行旋转
        if (pivotFront == Vector2Int.up) // 默认朝向（世界Z轴正方向）
        {
            return new Vector2Int(relativeCoor.x, relativeCoor.y);
        }
        else if (pivotFront == Vector2Int.right) // 旋转了90度
        {
            return new Vector2Int(-relativeCoor.y, relativeCoor.x);
        }
        else if (pivotFront == Vector2Int.down) // 旋转了180度
        {
            return new Vector2Int(-relativeCoor.x, -relativeCoor.y);
        }
        else if (pivotFront == Vector2Int.left) // 旋转了-90度
        {
            return new Vector2Int(relativeCoor.y, -relativeCoor.x);
        }

        // 如果方向不是正交方向，返回平移后的坐标
        return relativeCoor;
    }

    /// <summary>
    /// 这个是x东西相对于某物的坐标,worldPivotCoor是这个物的世界坐标,pivoFront是这个物的前方向
    /// </summary>
    /// <param name="relativeCoor"></param>
    /// <param name="worldPivotCoor"></param>
    /// <param name="pivotFront"></param>
    /// <returns></returns>
    public Vector2Int RelativeCoorToWorldCoor(Vector2Int relativeCoor, Vector2Int worldPivotCoor, Vector2Int pivotFront)
    {
        Vector2Int rotatedCoor;

        // 1. 反向旋转：根据相对坐标系的朝向进行旋转
        if (pivotFront == Vector2Int.up) // 默认朝向
        {
            rotatedCoor = new Vector2Int(relativeCoor.x, relativeCoor.y);
        }
        else if (pivotFront == Vector2Int.right) // 旋转了90度，反向旋转-90度
        {
            rotatedCoor = new Vector2Int(relativeCoor.y, -relativeCoor.x);
        }
        else if (pivotFront == Vector2Int.down) // 旋转了180度，反向旋转-180度
        {
            rotatedCoor = new Vector2Int(-relativeCoor.x, -relativeCoor.y);
        }
        else if (pivotFront == Vector2Int.left) // 旋转了-90度，反向旋转90度
        {
            rotatedCoor = new Vector2Int(-relativeCoor.y, relativeCoor.x);
        }
        else
        {
            rotatedCoor = relativeCoor; // 非正交方向，不进行旋转
        }
        // 2. 平移：加上原点的世界坐标
        return rotatedCoor + worldPivotCoor;
    }

    public Vector2Int GetDirectionFromRotation(Quaternion rotation)
    {
        // 获取旋转后的前方向量
        Vector3 forward = rotation * Vector3.forward;

        // 根据哪个分量绝对值更大来确定主方向，这样可以避免浮点数误差
        if (Mathf.Abs(forward.x) > Mathf.Abs(forward.z))
        {
            // 主方向是X轴
            return new Vector2Int(Mathf.RoundToInt(forward.x), 0);
        }
        else
        {
            // 主方向是Z轴
            return new Vector2Int(0, Mathf.RoundToInt(forward.z));
        }
    }
    #endregion

    public void AddTheRoom(string roomName, Vector2Int roomBigCoor,Vector2Int roomFront)
    {
        if (roomsDict.ContainsKey(roomBigCoor))
        {
            Debug.LogWarning("Room already occupy at: " + roomBigCoor);
            return;
        }
        AddressablesMgr.Instance.LoadAssetCoroutine<GameObject>(roomName, (handle) =>
        {
            GameObject theRoomObj = Instantiate(handle.Result);
            theRoomObj.transform.position = RoomWorldCoorToWorldPosBig(roomBigCoor);
            RoomBase theRoom = theRoomObj.GetComponent<RoomBase>();
            theRoom.transform.rotation = Quaternion.LookRotation(new Vector3(roomFront.x, 0, roomFront.y));
            theRoom.Init();
        });
    }

    public void RandomAddTheRoom(string roomName, Vector2Int roomBigCoor)
    {
        if (roomsDict.ContainsKey(roomBigCoor))
        {
            Debug.LogWarning("Room already occupy at: " + roomBigCoor);
            return;
        }
        AddressablesMgr.Instance.LoadAssetCoroutine<GameObject>(roomName, (handle) =>
        {
            GameObject theRoomObj = Instantiate(handle.Result);
            theRoomObj.transform.position = RoomWorldCoorToWorldPosBig(roomBigCoor);
            RoomBase theRoom = theRoomObj.GetComponent<RoomBase>();
            //从四个方向中随机选择一个方向作为前方
            List<Vector2Int> directions = new List<Vector2Int>{ Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            if(roomsDict.ContainsKey(roomBigCoor+Vector2Int.up))
            {
                directions.Remove(Vector2Int.up);
            }
            if (roomsDict.ContainsKey(roomBigCoor + Vector2Int.right))
            {
                directions.Remove(Vector2Int.right);
            }
            if (roomsDict.ContainsKey(roomBigCoor + Vector2Int.down))
            {
                directions.Remove(Vector2Int.down);
            }
            if (roomsDict.ContainsKey(roomBigCoor + Vector2Int.left))
            {
                directions.Remove(Vector2Int.left);
            }

            if(directions.Count == 0)
            {
                //特殊处理,这个房间没有可行走的方向
                return;
            }
            int randomIndex = Random.Range(0, directions.Count);
            Vector2Int randomDirection = directions[randomIndex];

            theRoom.transform.rotation = Quaternion.LookRotation(new Vector3(randomDirection.x, 0, randomDirection.y));
            theRoom.Init();
        });
    }

    #region 生命周期函数
    private void OnEnable()
    {
        roomsDict.Add(Vector2Int.zero, GameObject.Find("RoomStart").GetComponent<RoomBase>());
        //现阶段这么写了
        if (currentRoom != null)
            currentRoom.Init();
    }
    private void Update()
    {
        
    }
    #endregion


    #region 可视化调试
    public bool showGrid = true;
    public Color gridColor = Color.gray;
    public Vector2Int gridSize = new Vector2Int(20, 20);
    private void OnDrawGizmos()
    {
        if (!showGrid) return;

        Gizmos.color = gridColor;

        //水平线
        for (float x = -gridSize.x / 2-cellSize/2; x <= gridSize.x / 2 - cellSize/2; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, -gridSize.y / 2 * cellSize);
            Vector3 end = new Vector3(x * cellSize, 0, gridSize.y / 2 * cellSize);
            Gizmos.DrawLine(start, end);
        }
        //垂直线
        for (float z = -gridSize.y / 2 - cellSize/2; z <= gridSize.y / 2 - cellSize/2; z++)
        {
            Vector3 start = new Vector3(-gridSize.x / 2 * cellSize, 0, z * cellSize);
            Vector3 end = new Vector3(gridSize.x / 2 * cellSize, 0, z * cellSize);
            Gizmos.DrawLine(start, end);
        }

        Gizmos.color = Color.red;

        // 绘制所有不可行走的坐标
        foreach (var coor in allUnwalkableCoor)
        {
            Vector3 pos = WorldCoorToWorldPos(coor);
            Gizmos.DrawCube(pos, Vector3.one * 0.3f);
        }

    }
    #endregion
}
