using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    //用于得到不可行走的格子信息
    public Room theRoom;
    public Vector3 toPos;
    public bool isToSceneObj = false;
    //public bool isInSceneObj = false;
    public InteractableSceneObj toSceneObj=null;
    public InteractableSceneObj inSceneObj=null;
    
    private void Start()
    {
        toPos = transform.position;
        EventCenter.Instance.AddEventListener<InteractableSceneObj>(E_EventType.E_PlayerEnterInteractableSceneObjFront, OnPlayerEnterInteractableSceneObjFront);
        EventCenter.Instance.AddEventListener<InteractableSceneObj>(E_EventType.E_PlayerExitInteractableSceneObjFront, OnPlayerExitInteractableSceneObjFront);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<InteractableSceneObj>(E_EventType.E_PlayerEnterInteractableSceneObjFront, OnPlayerEnterInteractableSceneObjFront);
        EventCenter.Instance.RemoveEventListener<InteractableSceneObj>(E_EventType.E_PlayerExitInteractableSceneObjFront, OnPlayerExitInteractableSceneObjFront);
    }
    
    void Move()
    {
        //如果要到的区域不能够行走,不动
        if (theRoom.unWalkableAreas.Contains(new Vector2Int((int)toPos.x,(int)toPos.z)))
        {
            toPos = transform.position;
            return;
        }
        transform.DOMove(toPos, 0.2f).SetEase(Ease.OutCubic);
    }
    //事件响应
    public void OnPlayerEnterInteractableSceneObjFront(InteractableSceneObj toSceneObj)
    {
        isToSceneObj = true;
        this.toSceneObj = toSceneObj;
    }
    public void OnPlayerExitInteractableSceneObjFront(InteractableSceneObj toSceneObj)
    {
        isToSceneObj = false;
        if(this.toSceneObj == toSceneObj)
            this.toSceneObj = null;
    }
    //输入事件
    void OnMove(InputValue value)
    {
        bool isForward = value.Get<float>() > 0;
        

        if (inSceneObj!=null)
        {
            if (isForward)
            {
                toSceneObj.Enter();
            }
            else
            {
                toSceneObj.Exit();
                if(toSceneObj.EnterLevel == 0)
                {
                    inSceneObj = null;
                }
            }
            return;
        }
        if (isToSceneObj)
        {
            bool isFaceToSceneObj = Vector3.Dot((toSceneObj.transform.position - transform.position).normalized, transform.forward) > 0.7;
            if (isFaceToSceneObj)
            {
                if (isForward)
                {
                    toSceneObj.Enter();
                    inSceneObj = toSceneObj;
                }
                else
                {
                    toPos = transform.position - transform.forward;
                    Move();
                }
            }
            else
            {
                toPos = transform.position + (isForward?1:-1)*transform.forward;
                Move();
            }
            return;
        }
        toPos = transform.position + (isForward ? 1 : -1) * transform.forward;
        Move();
    }
}


    
