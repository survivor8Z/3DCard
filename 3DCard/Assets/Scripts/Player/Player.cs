using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //在哪个房间
    public Room currentRoom;
    public Vector2Int coordinatesInRoom
    {
        get
        {
            Vector3 RelativePos = transform.position -currentRoom.transform.position;
            return new Vector2Int((int)(RelativePos.x),(int)(RelativePos.z));
        }
    }

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
        
    }
}
