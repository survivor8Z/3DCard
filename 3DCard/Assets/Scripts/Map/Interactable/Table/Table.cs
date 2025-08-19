using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : InteractableSceneObj
{
    public CinemachineVirtualCamera theVC;

    //拖拽位置相关
    public Transform dragPoint ;//之后可能限制一下位置
    public Transform tableCardTransformParent;
    public float dragPointK;//调整系数
    //下面是管理卡牌相关
    public TableCardsControl tableCardsControl;

    private void Awake()
    {
        tableCardsControl = GetComponent<TableCardsControl>();
    }
    public override void Enter()
    {
        if (EnterLevel == EnterMaxLevel) return;
        EnterLevel++;
        if (EnterLevel == 1)
        {
            EventCenter.Instance.EventTrigger<InteractableSceneObj>(E_EventType.E_PlayerEnterInteractableSceneObj, this);
        }
        Set();
    }

    public override void Exit()
    {
        EnterLevel--;
        if (EnterLevel < 0)
        {
            EnterLevel = 0;
        }
        if(EnterLevel == 0)
        {
            EventCenter.Instance.EventTrigger<InteractableSceneObj>(E_EventType.E_PlayerExitInteractableSceneObj, this);
        }
        Set();
    }
    public void Set()
    {
        switch (EnterLevel)
        {
            case 0:
                VCMgr.Instance.SetDefaultVC();
                break;
            case 1:
                VCMgr.Instance.SetCurrentVC(theVC);
                break;
        }
    }
    public void UpdateTablePoint(Vector3 pos)
    {
        dragPoint.position = pos+Vector3.down*dragPointK;
        //设置卡牌的父节点

    }
}
