using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : SingletonMono<MapMgr>
{
    public GameObject roomPre;
    public List<Room> rooms = new List<Room>();//场景中的房间列表
    public Room currentRoom;
    private void Start()
    {
        
    }
}
