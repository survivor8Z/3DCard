using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 这个是用来场景交互的,就是会调整摄像机视角之类的
/// </summary>
public class InteractableSceneObj : InteractableObject
{

    public Transform pickPoint;
    //进入的层级
    public int EnterLevel = 0;
    public int EnterMaxLevel = 1;

    /// <summary>
    /// 用于触发摄像机,等切换
    /// </summary>
    public virtual void Enter()
    {

    }

    /// <summary>
    /// 用于回退
    /// </summary>
    public virtual void Exit()
    {

    }
}
