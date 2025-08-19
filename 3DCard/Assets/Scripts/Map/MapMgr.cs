using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : SingletonMono<MapMgr>
{
    public RoomBase currentRoom;//��Player���︳ֵ��
    [ShowInInspector]public Dictionary<Vector2Int, RoomBase> roomsDict=new();//�Ǵ�����,��RoomSizeΪ��λ������ϵ
    public readonly int RoomSize = 10;//����������
    


    [ShowInInspector]public HashSet<Vector2Int> allUnwalkableCoor = new();
    public readonly int cellSize = 1;


    #region ����ת��
    /// <summary>
    /// ������
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public Vector2Int WorldPosToRoomWorldCoorBig(Vector3 worldPos)
    {
        // ������������Է����С��Ȼ����������
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / RoomSize),
            Mathf.RoundToInt(worldPos.z / RoomSize)
        );
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="roomWorldCoor"></param>
    /// <returns></returns>
    public Vector3 RoomWorldCoorToWorldPosBig(Vector2Int roomWorldCoor)
    {
        // ����������������Է����С
        return new Vector3(
            roomWorldCoor.x * RoomSize,
            0,
            roomWorldCoor.y * RoomSize
        );
    }

    public Vector2Int WorldPosToWorldCoor(Vector3 worldPos)
    {
        // ������������Ե�Ԫ���С��Ȼ����������
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.z / cellSize)
        );
    }

    public Vector3 WorldCoorToWorldPos(Vector2Int worldCoor)
    {
        // ������������Ե�Ԫ���С
        return new Vector3(
            worldCoor.x * cellSize,
            0,
            worldCoor.y * cellSize
        );
    }

    public Vector2Int WorldCoorToRelativeCoor(Vector2Int worldCoor,Vector2Int worldPivotCoor,Vector2Int pivotFront)
    {
        // 1. ƽ�ƣ����������ԭ�����������
        Vector2Int relativeCoor = worldCoor - worldPivotCoor;
        // 2. ��ת�������������ϵ�ĳ��������ת
        if (pivotFront == Vector2Int.up) // Ĭ�ϳ�������Z��������
        {
            return new Vector2Int(relativeCoor.x, relativeCoor.y);
        }
        else if (pivotFront == Vector2Int.right) // ��ת��90��
        {
            return new Vector2Int(-relativeCoor.y, relativeCoor.x);
        }
        else if (pivotFront == Vector2Int.down) // ��ת��180��
        {
            return new Vector2Int(-relativeCoor.x, -relativeCoor.y);
        }
        else if (pivotFront == Vector2Int.left) // ��ת��-90��
        {
            return new Vector2Int(relativeCoor.y, -relativeCoor.x);
        }

        // ����������������򣬷���ƽ�ƺ������
        return relativeCoor;
    }

    /// <summary>
    /// �����x���������ĳ�������,worldPivotCoor����������������,pivoFront��������ǰ����
    /// </summary>
    /// <param name="relativeCoor"></param>
    /// <param name="worldPivotCoor"></param>
    /// <param name="pivotFront"></param>
    /// <returns></returns>
    public Vector2Int RelativeCoorToWorldCoor(Vector2Int relativeCoor, Vector2Int worldPivotCoor, Vector2Int pivotFront)
    {
        Vector2Int rotatedCoor;

        // 1. ������ת�������������ϵ�ĳ��������ת
        if (pivotFront == Vector2Int.up) // Ĭ�ϳ���
        {
            rotatedCoor = new Vector2Int(relativeCoor.x, relativeCoor.y);
        }
        else if (pivotFront == Vector2Int.right) // ��ת��90�ȣ�������ת-90��
        {
            rotatedCoor = new Vector2Int(relativeCoor.y, -relativeCoor.x);
        }
        else if (pivotFront == Vector2Int.down) // ��ת��180�ȣ�������ת-180��
        {
            rotatedCoor = new Vector2Int(-relativeCoor.x, -relativeCoor.y);
        }
        else if (pivotFront == Vector2Int.left) // ��ת��-90�ȣ�������ת90��
        {
            rotatedCoor = new Vector2Int(-relativeCoor.y, relativeCoor.x);
        }
        else
        {
            rotatedCoor = relativeCoor; // ���������򣬲�������ת
        }
        // 2. ƽ�ƣ�����ԭ�����������
        return rotatedCoor + worldPivotCoor;
    }

    public Vector2Int GetDirectionFromRotation(Quaternion rotation)
    {
        // ��ȡ��ת���ǰ������
        Vector3 forward = rotation * Vector3.forward;

        // �����ĸ���������ֵ������ȷ���������������Ա��⸡�������
        if (Mathf.Abs(forward.x) > Mathf.Abs(forward.z))
        {
            // ��������X��
            return new Vector2Int(Mathf.RoundToInt(forward.x), 0);
        }
        else
        {
            // ��������Z��
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
            //���ĸ����������ѡ��һ��������Ϊǰ��
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
                //���⴦��,�������û�п����ߵķ���
                return;
            }
            int randomIndex = Random.Range(0, directions.Count);
            Vector2Int randomDirection = directions[randomIndex];

            theRoom.transform.rotation = Quaternion.LookRotation(new Vector3(randomDirection.x, 0, randomDirection.y));
            theRoom.Init();
        });
    }

    #region �������ں���
    private void OnEnable()
    {
        roomsDict.Add(Vector2Int.zero, GameObject.Find("RoomStart").GetComponent<RoomBase>());
        //�ֽ׶���ôд��
        if (currentRoom != null)
            currentRoom.Init();
    }
    private void Update()
    {
        
    }
    #endregion


    #region ���ӻ�����
    public bool showGrid = true;
    public Color gridColor = Color.gray;
    public Vector2Int gridSize = new Vector2Int(20, 20);
    private void OnDrawGizmos()
    {
        if (!showGrid) return;

        Gizmos.color = gridColor;

        //ˮƽ��
        for (float x = -gridSize.x / 2-cellSize/2; x <= gridSize.x / 2 - cellSize/2; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, -gridSize.y / 2 * cellSize);
            Vector3 end = new Vector3(x * cellSize, 0, gridSize.y / 2 * cellSize);
            Gizmos.DrawLine(start, end);
        }
        //��ֱ��
        for (float z = -gridSize.y / 2 - cellSize/2; z <= gridSize.y / 2 - cellSize/2; z++)
        {
            Vector3 start = new Vector3(-gridSize.x / 2 * cellSize, 0, z * cellSize);
            Vector3 end = new Vector3(gridSize.x / 2 * cellSize, 0, z * cellSize);
            Gizmos.DrawLine(start, end);
        }

        Gizmos.color = Color.red;

        // �������в������ߵ�����
        foreach (var coor in allUnwalkableCoor)
        {
            Vector3 pos = WorldCoorToWorldPos(coor);
            Gizmos.DrawCube(pos, Vector3.one * 0.3f);
        }

    }
    #endregion
}
