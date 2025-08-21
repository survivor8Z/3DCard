using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour,IEndDragHandler
{
    [SerializeField] Transform target;

    

    public void OnEndDrag(PointerEventData eventData)
    {
        print("OnEndDrag");
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            print("GetMouseButtonUp");
        }
        
    }
}
