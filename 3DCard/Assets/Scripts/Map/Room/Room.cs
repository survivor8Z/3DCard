using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int index;
    public int roomSizeX = 10;
    public int roomSizeY = 10;
    public HashSet<Vector2Int> unWalkableAreas = new HashSet<Vector2Int>();
    public List<InteractableObject> roomInteractableObjects = new List<InteractableObject>();

    public Table table;

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        for (int i = 0; i < roomInteractableObjects.Count; i++)
        {
            for(int j= 0; j < roomInteractableObjects[i].obstacleCoor.Count; j++)
            {
                unWalkableAreas.Add(roomInteractableObjects[i].obstacleCoor[j]);
            }
        }

    }
}
