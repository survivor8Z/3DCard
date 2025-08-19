using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardVisual : MonoBehaviour
{
    private TableCardsControl tableCardControl=>thaTableCard.table.tableCardsControl;
    private TableCardBase thaTableCard;
    public Vector3 toPos;
    [HideInInspector] public HandCardVisual theHandCardVisual;


    //用于初始化
    #region 生命周期函数
    private void Awake()
    {
        thaTableCard = GetComponent<TableCardBase>();
        theHandCardVisual = GetComponent<HandCardVisual>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        SetPositon();
    }
    #endregion

    public void Init()
    {
        toPos = thaTableCard.table.dragPoint.position;
        followVelocity = theHandCardVisual.followVelocity;
    }


    Vector3 followVelocity;
    public void SetPositon()
    {
        if (false)
        {
            return; 
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            toPos,
            ref followVelocity,
            0.1f
        );
    }
    public void SetToPosition()
    {

    }
}
