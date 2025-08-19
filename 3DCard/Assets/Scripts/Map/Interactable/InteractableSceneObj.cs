using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �������������������,���ǻ����������ӽ�֮���
/// </summary>
public class InteractableSceneObj : InteractableObject
{

    public Transform pickPoint;
    //����Ĳ㼶
    public int EnterLevel = 0;
    public int EnterMaxLevel = 1;

    /// <summary>
    /// ���ڴ��������,���л�
    /// </summary>
    public virtual void Enter()
    {

    }

    /// <summary>
    /// ���ڻ���
    /// </summary>
    public virtual void Exit()
    {

    }
}
