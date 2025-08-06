using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardVisual : MonoBehaviour
{
    [SerializeField]private TableCardBase thaTableCard;
    public Vector3 toPos;//��ͨ״̬��Ҫȥ��λ��
    public HandCardVisual theHandCardVisual;


    //���ڳ�ʼ��
    #region �������ں���
    private void Awake()
    {
        
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
}
