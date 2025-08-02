using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public List<Vector2Int> obstacleCoor = new List<Vector2Int>();//ÔÚ±à¼­Æ÷Àï¸ã°É
    




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < obstacleCoor.Count; i++)
        {
            Vector3 pos = new Vector3(obstacleCoor[i].x, 0, obstacleCoor[i].y);
            Gizmos.DrawCube(pos, Vector3.one * 0.5f);
        }
    }
}
