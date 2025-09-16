using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public RoomBase inRoom; //所在的房间,生成时赋值
    public Vector2Int worldPivotCoor => MapMgr.Instance.WorldPosToWorldCoor(transform.position);
    public Vector2Int pivotFront => MapMgr.Instance.GetDirectionFromRotation(transform.rotation);


    public List<Vector2Int> obstacleRelativeCoor = new List<Vector2Int>();//相对自己的坐标
    public Transform pivotTransform;//

    public Collider boxCollider;


    //IInteractable接口
    public int id;
    public int InteractableID
    {
        get
        {
            return id;
        }
    }
    public virtual void React()
    {
        Debug.Log("InteractableObject React");
    }

    public void SetPivot()
    {
        pivotTransform = transform.Find("Pivot");
    }

    /// <summary>
    /// 旋转提前设置好了
    /// </summary>
    /// <param name="theRoom"></param>
    /// <param name="worldCoor"></param>
    public void SetToRoom(RoomBase createInRoom,Vector2Int worldCoor)
    {
        inRoom = createInRoom;
        
    }


    public virtual void Init()
    {
        SetPivot();
    }


    public virtual void Delete()
    {
        Destroy(gameObject);
    }

    #region 生命周期函数
    private void Awake()
    {
        boxCollider = GetComponent<Collider>();
    }

    #endregion











    private void OnDrawGizmos()
    {
        if (MapMgr.Instance == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        foreach (var coor in obstacleRelativeCoor)
        {
            Vector3 worldPos = MapMgr.Instance.WorldCoorToWorldPos(MapMgr.Instance.RelativeCoorToWorldCoor(
                coor, worldPivotCoor, pivotFront));
            Gizmos.DrawCube(worldPos, Vector3.one * 0.3f);
        }
    }
}
