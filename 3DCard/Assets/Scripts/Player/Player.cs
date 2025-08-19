using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInteract playerInteract;
    public PlayerMove playerMove;
    public PlayerTurn playerTurn;
    public CameraController cameraController;

    private void Awake()
    {
        playerInteract = GetComponent<PlayerInteract>();
        playerMove = GetComponent<PlayerMove>();
        playerTurn = GetComponent<PlayerTurn>();
        cameraController = GetComponent<CameraController>();
    }
    private void Update()
    {
        //设置当前房间
        MapMgr.Instance.currentRoom 
            = MapMgr.Instance.roomsDict[MapMgr.Instance.WorldPosToRoomWorldCoorBig(transform.position)];

        //test
        if (Input.GetKeyDown(KeyCode.O))
        {
            MapMgr.Instance.currentRoom.door.ToggleDoor();
        }
    }
}
