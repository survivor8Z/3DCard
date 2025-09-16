using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardVisual : MonoBehaviour
{
    private TableCardsControl tableCardControl=>thaTableCard.table.tableCardsControl;
    private TableCardBase thaTableCard;
    public Transform stackPointTransform;//堆叠的点,用于设置toPos
    public Vector3 toPos;
    public float offsetDragAbove=0.05f;
    [HideInInspector]public float offsetDragAboveK;//拖拽时上下的偏移量
    [HideInInspector] public HandCardVisual theHandCardVisual;

    //桌牌间斥力相关,之后有时间再做
    public bool isInThePlayerRoom=false;//如果玩家不在房间就不进行斥力计算
    public List<Vector3> nearByTableCardCenterList = new();

    //用于初始化
    #region 生命周期函数
    private void Awake()
    {
        thaTableCard = GetComponent<TableCardBase>();
        theHandCardVisual = GetComponent<HandCardVisual>();
    }
    private void OnEnable()
    {
        SetScale();
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
        //if (false)
        //{
        //    return; 
        //}

        transform.position = Vector3.SmoothDamp(
            transform.position,
            toPos+ offsetDragAboveK * Vector3.up,
            ref followVelocity,
            0.1f
        );
    }
    //没做斥力
    public void SetToPos()
    {
        if (isInThePlayerRoom)
        {
            if (stackPointTransform != null)
            {
                toPos = stackPointTransform.position;
            }
        }
        else
        {
            if (stackPointTransform != null)
            {
                toPos = stackPointTransform.position;
            }
        }

        
    }
    //防止大小不一
    private void SetScale()
    {
        transform.localScale = Vector3.one;
    }
}
