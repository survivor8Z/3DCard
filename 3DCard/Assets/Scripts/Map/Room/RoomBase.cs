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
    public List<Vector2Int> roomRelativeOccupyCoor = new();//是大坐标
    public Vector2Int roomBigWorldPivotCoor =>MapMgr.Instance.WorldPosToRoomWorldCoorBig(pivotTransform.position); //大坐标

    public Vector2Int roomFront => MapMgr.Instance.GetDirectionFromRotation(transform.rotation); //房间前方的方向,大坐标系下的方向

    //房间内
    public Vector2Int roomWorldPivotCoor => MapMgr.Instance.WorldPosToWorldCoor(pivotTransform.position);
    [ShowInInspector] public HashSet<Vector2Int> unWalkableWorldCoor = new HashSet<Vector2Int>();
    public List<InteractableObject> roomInteractableObjects = new List<InteractableObject>();
    //锚点
    public Transform pivotTransform;
    //墙的父物体
    public Transform wallParent;

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
        foreach(var bigCoor in roomRelativeOccupyCoor)
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
            AddWall("NormalWall",Vector2Int.up * 5, (wallBase) => AddTheObstacle(wallBase));
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(transform.right)))
        {
            print("right");
            AddWall("NormalWall", Vector2Int.right * 5, (wallBase) => AddTheObstacle(wallBase));
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(-transform.forward)))
        {
            print("back");
            AddWall("NormalWall", Vector2Int.down * 5, (wallBase) => AddTheObstacle(wallBase));
        }
        if (!MapMgr.Instance.roomsDict.ContainsKey(roomBigWorldPivotCoor + MapMgr.Instance.WorldPosToWorldCoor(-transform.right)))
        {
            print("left");
            AddWall("NormalWall", Vector2Int.left * 5, (wallBase) => AddTheObstacle(wallBase));
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
            AddWall("DoorWall", wallRelativeRoomPivotCoor, (wallBase) => AddTheObstacle(wallBase));
        }
    }
    #endregion
    public virtual void SetInteractableObject()
    {
        table = GetComponentInChildren<Table>();
        door = GetComponentInChildren<Door>();

        
    }
    /// <summary>
    /// 删除房间的占用坐标,大坐标
    /// </summary>
    public void DelBigOccupy()
    {
        foreach (var bigCoor in roomRelativeOccupyCoor)
        {
            MapMgr.Instance.roomsDict.Remove(MapMgr.Instance.RelativeCoorToWorldCoor(
                bigCoor
                , roomBigWorldPivotCoor,
                MapMgr.Instance.GetDirectionFromRotation(transform.rotation)));
        }
    }
    public virtual void Init()
    {
        SetPivot();
        SetBigOccupy();
        SetWall();
        SetInteractableObject();
        for (int i = 0; i < roomInteractableObjects.Count; i++)
        {
            RegisterInteractableObject(roomInteractableObjects[i]);
        }
    }

    

    /// <summary>
    /// 添加一个可交互的物体到房间中
    /// 通过Addressables加载
    /// </summary>
    /// <param name="name"></param>
    public void AddInteractableObject(string name,Vector2Int roomRelativeCellCoor)
    {
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(name, (handle) =>
        {
            GameObject interactableObj = handle.Result;
            if (interactableObj != null)
            {
                GameObject obj = Instantiate(interactableObj, transform);
                obj.transform.localPosition = MapMgr.Instance.WorldCoorToWorldPos(roomRelativeCellCoor);
                InteractableObject interactable = obj.GetComponent<InteractableObject>();
                RegisterInteractableObject(interactable);
            }
            else
            {
                Debug.LogWarning("Failed to load interactable object: " + name);
            }
        });

    }

    private void RegisterInteractableObject(InteractableObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Trying to register a null InteractableObject.");
            return;
        }

        obj.inRoom = this; //设置所在房间
        if (!roomInteractableObjects.Contains(obj))
        {
            roomInteractableObjects.Add(obj);
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
            unWalkableWorldCoor.Add(obstacleWorldCoor);
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
                unWalkableWorldCoor.Remove(obstacleWorldCoor);
            }
            Destroy(interactableObj.gameObject);
        }
        else
        {
            Debug.LogWarning("InteractableObject not found in the room: " + interactableObj.name);
        }
    }

    /// <summary>
    /// 用于需要动态删除障碍物的情况
    /// </summary>
    /// <param name="interactableObject"></param>
    public void DelTheObstacle(InteractableObject interactableObject)
    {

        foreach (var coor in interactableObject.obstacleRelativeCoor)
        {
            Vector2Int obstacleWorldCoor = MapMgr.Instance.RelativeCoorToWorldCoor
            (
                coor,
                interactableObject.worldPivotCoor,
                interactableObject.pivotFront
            );
            MapMgr.Instance.allUnwalkableCoor.Remove(obstacleWorldCoor);
            unWalkableWorldCoor.Remove(obstacleWorldCoor);
        }
        


    }

    /// <summary>
    /// 用于需要动态添加障碍物的情况`
    /// </summary>
    /// <param name="interactableObject"></param>
    public void AddTheObstacle(InteractableObject interactableObject)
    {
        foreach (var coor in interactableObject.obstacleRelativeCoor)
        {
            Vector2Int obstacleWorldCoor = MapMgr.Instance.RelativeCoorToWorldCoor
            (
                coor,
                interactableObject.worldPivotCoor,
                interactableObject.pivotFront
            );
            MapMgr.Instance.allUnwalkableCoor.Add(obstacleWorldCoor);
            unWalkableWorldCoor.Add(obstacleWorldCoor);
        }
    }

    
    
}
