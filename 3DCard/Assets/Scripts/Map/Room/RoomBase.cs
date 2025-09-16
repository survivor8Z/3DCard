using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class RoomBase : MonoBehaviour
{
    [HideInInspector]public static int INDEX = 1;
    public int index;
    //对于地图上
    public List<Vector2Int> roomRelativeOccupyCoorBig = new();//是大坐标
    public Vector2Int roomBigWorldPivotCoor =>MapMgr.Instance.WorldPosToRoomWorldCoorBig(pivotTransform.position); //大坐标

    public Vector2Int roomFront => MapMgr.Instance.GetDirectionFromRotation(transform.rotation); //房间前方的方向,大坐标系下的方向

    //房间内
    public Vector2Int roomWorldPivotCoor => MapMgr.Instance.WorldPosToWorldCoor(pivotTransform.position);
    public List<InteractableObject> roomInteractableObjects = new List<InteractableObject>();
    //锚点
    public Transform pivotTransform;
    //墙的父物体
    public Transform wallParent;

    //敌人管理

    //一些固定场景物体
    public Table table;
    public Door door;
    [ShowInInspector]public Dictionary<Vector2Int, WallBase> wallDict=new();


    #region 生命周期函数
    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        
    }
    private void Start()
    {
        
    }
    #endregion

    #region 初始化相关
    public void SetPivot()
    {
        pivotTransform = transform.Find("Pivot");

        wallParent = transform.Find("WallParent");
    }
    /// <summary>
    /// 设置房间的占用坐标,大坐标
    /// </summary>
    public void SetBigOccupy()
    {
        foreach(var bigCoor in roomRelativeOccupyCoorBig)
        {
            Vector2Int temp = MapMgr.Instance.RelativeCoorToWorldCoor(
                bigCoor,
                roomBigWorldPivotCoor,
                MapMgr.Instance.GetDirectionFromRotation(transform.rotation));

            if (MapMgr.Instance.roomsDict.ContainsKey(temp))
            {
                Debug.Log(temp+"房间坐标冲突,请检查房间占用坐标设置");
                continue;
            }
            MapMgr.Instance.roomsDict.Add(temp,this);
        }
        
    }
    /// <summary>
    /// 删除房间的占用坐标,大坐标
    /// </summary>
    public void DelBigOccupy()
    {
        foreach (var bigCoor in roomRelativeOccupyCoorBig)
        {
            MapMgr.Instance.roomsDict.Remove(MapMgr.Instance.RelativeCoorToWorldCoor(
                bigCoor
                , roomBigWorldPivotCoor,
                MapMgr.Instance.GetDirectionFromRotation(transform.rotation)));
        }
    }

    #region 墙相关

    /// <summary>
    /// 用于初始化时,如果已经设置的墙壁则不设置墙壁
    /// </summary>
    public virtual void SetWall()
    {
        //遍历四周房间
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(transform.forward)))
        {
            print("forward");
            AddWall("NormalWall",Vector2Int.up * 5, (wallBase) => MapMgr.Instance.AddTheObstacle(wallBase));
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(transform.right)))
        {
            print("right");
            AddWall("NormalWall", Vector2Int.right * 5, (wallBase) => MapMgr.Instance.AddTheObstacle(wallBase));
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(-transform.forward)))
        {
            print("back");
            AddWall("NormalWall", Vector2Int.down * 5, (wallBase) => MapMgr.Instance.AddTheObstacle(wallBase));
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(-transform.right)))
        {
            print("left");
            AddWall("NormalWall", Vector2Int.left * 5, (wallBase) => MapMgr.Instance.AddTheObstacle(wallBase));
        }
    }

    public void AddWall(string wallName,Vector2Int wallRelativeRoomPivotCoor,UnityAction<WallBase> callBack)
    {
        if(wallDict.ContainsKey(wallRelativeRoomPivotCoor))
        {
            Debug.LogWarning("Wall already exists at: " + wallRelativeRoomPivotCoor);
            callBack?.Invoke(wallDict[wallRelativeRoomPivotCoor]);
            return;
        }
        PoolMgr.Instance.GetObjByCoroutine(wallName, (obj) =>
        {
            
            Vector2Int wallWorldCoor = MapMgr.Instance.RelativeCoorToWorldCoor
                (
                    wallRelativeRoomPivotCoor,
                    roomWorldPivotCoor,
                    MapMgr.Instance.WorldPosToWorldCoor(transform.forward)
                );
            obj.transform.position = MapMgr.Instance.WorldCoorToWorldPos(wallWorldCoor);
            Vector3 dir = obj.transform.position - pivotTransform.position;
            obj.transform.rotation = Quaternion.LookRotation(dir);
            obj.transform.SetParent(wallParent);

            WallBase theWall = obj.GetComponent<WallBase>();
            wallDict.Add(wallRelativeRoomPivotCoor, theWall);
            callBack?.Invoke(theWall);
        });
    }
    

    public void WallToDoorWall(Vector2Int wallRelativeRoomPivotCoor)
    {
        if(wallDict.ContainsKey(wallRelativeRoomPivotCoor))
        {
            WallBase wall = wallDict[wallRelativeRoomPivotCoor];
            if (wall is WallDoor)
            {
                Debug.LogWarning("The wall is already a door wall.");
                return;
            }
            PoolMgr.Instance.GetObjByCoroutine("DoorWall", (obj) =>
            {
                obj.transform.SetParent(wallParent);
                Vector2Int wallWorldCoor = MapMgr.Instance.RelativeCoorToWorldCoor
                    (
                        wallRelativeRoomPivotCoor,
                        roomWorldPivotCoor,
                        MapMgr.Instance.GetDirectionFromRotation(transform.rotation)
                    );
                obj.transform.position = MapMgr.Instance.WorldCoorToWorldPos(wallWorldCoor);
                Vector3 dir = obj.transform.position - pivotTransform.position;
                obj.transform.rotation = Quaternion.LookRotation(dir);
                WallDoor doorWall = obj.GetComponent<WallDoor>();
                wallDict[wallRelativeRoomPivotCoor] = doorWall;
                PoolMgr.Instance.PushObj(wall.gameObject);
            });
        }
        else
        {
            AddWall("DoorWall", wallRelativeRoomPivotCoor, (wallBase) => MapMgr.Instance.AddTheObstacle(wallBase));
        }
    }
    #endregion
    public virtual void SetInteractableObject()
    {
        table = GetComponentInChildren<Table>();
        door = GetComponentInChildren<Door>();

        AddInteractableObject(table);
        AddInteractableObject(door);

    }
    public virtual void SetEnemy()
    {
        //如果需要添加则添加,现在是测试
        //RandomAddEnemy("TestEnemy");
        if(MapMgr.Instance.roomsDict.Count > 1)
            AddTheEnemy("TestEnemy", Vector2Int.zero);
    }
    public virtual void Init()
    {
        SetPivot();
        SetBigOccupy();
        SetWall();
        SetInteractableObject();
        SetEnemy();
    }
    #endregion


    #region 可交互物体相关
    /// <summary>
    /// 注册一个可交互物体到房间中,并设置其所在房间
    /// </summary>
    /// <param name="obj"></param>
    public void AddInteractableObject(InteractableObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Trying to register a null InteractableObject.");
            return;
        }

        print("添加可交互物体: " + obj.name + " 到房间: " + gameObject.name);
        obj.inRoom = this; //设置所在房间
        obj.transform.SetParent(transform); //设置父物体为房间
        if (!roomInteractableObjects.Contains(obj))
        {
            roomInteractableObjects.Add(obj);
            
        }
        else
        {
            Debug.LogWarning("重复添加");
        }

        for (int j = 0; j < obj.obstacleRelativeCoor.Count; j++)
        {
            Vector2Int obstacleWorldCoor = MapMgr.Instance.RelativeCoorToWorldCoor
                (
                    obj.obstacleRelativeCoor[j],
                    obj.worldPivotCoor,
                    obj.pivotFront
                );

            MapMgr.Instance.allUnwalkableCoor.Add(obstacleWorldCoor);
        }
    }

    /// <summary>
    /// 删除一个可交互物体
    /// </summary>
    /// <param name="interactableObj"></param>
    public void DelInteractableObject(InteractableObject interactableObj)
    {
        if (roomInteractableObjects.Contains(interactableObj))
        {
            roomInteractableObjects.Remove(interactableObj);
            foreach (var coor in interactableObj.obstacleRelativeCoor)
            {
                Vector2Int obstacleWorldCoor = MapMgr.Instance.RelativeCoorToWorldCoor
                (
                    coor,
                    interactableObj.worldPivotCoor,
                    interactableObj.pivotFront
                );
                MapMgr.Instance.allUnwalkableCoor.Remove(obstacleWorldCoor);
            }
            interactableObj.Delete();
        }
        else
        {
            Debug.LogWarning("InteractableObject not found in the room: " + interactableObj.name);
        }
    }

    #endregion


    #region 敌人管理
    /// <summary>
    /// 创建一个敌人并添加到房间中
    /// </summary>
    /// <param name="name"></param>
    /// <param name="relativeRoomCoor"></param>
    public void AddTheEnemy(string name,Vector2Int relativeRoomCoor)
    {
        PoolMgr.Instance.GetObjByCoroutine(name, (obj) =>
        {
            obj.transform.SetParent(transform);
            Vector2Int enemyWorldCoor = MapMgr.Instance.RelativeCoorToWorldCoor(
                relativeRoomCoor,
                roomWorldPivotCoor,
                MapMgr.Instance.WorldPosToWorldCoor(transform.forward));
            obj.transform.position = MapMgr.Instance.WorldCoorToWorldPos(enemyWorldCoor);
            EnemyBase enemy = obj.GetComponent<EnemyBase>();
            enemy.inRoom = this;
            enemy.Init();
            AddInteractableObject(enemy);
        });
    }

    public void RandomAddEnemy(string name)
    {
        //在大坐标获取一个随机坐标
        int randomBigIndex = Random.Range(0, roomRelativeOccupyCoorBig.Count);
        Vector2Int roomBigCoor = roomRelativeOccupyCoorBig[randomBigIndex];
        //在大坐标所指的格子中随机一个格子
        //现在直接现在0,1生成
        AddTheEnemy(name,
            MapMgr.Instance.WorldPosToWorldCoor(MapMgr.Instance.RoomWorldCoorToWorldPosBig(roomBigCoor))+Vector2Int.up);

    }

    #endregion
}
