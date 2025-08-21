using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardVisual : MonoBehaviour
{
    private TableCardsControl tableCardControl=>thaTableCard.table.tableCardsControl;
    private TableCardBase thaTableCard;
    public Transform stackPointTransform;//堆叠的点,用于设置toPos
    public Vector3 toPos;
    public float offsetDragAbove=0.1f;//在面板配置
    [HideInInspector]public float offsetDragAboveK;//拖拽时上下的偏移量
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
        SetToPos();
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
            toPos+ offsetDragAboveK * Vector3.up,
            ref followVelocity,
            0.1f
        );
    }
    public void SetToPos()
    {
        if (stackPointTransform != null)
        {
            toPos = stackPointTransform.position;
        }
    }
}
