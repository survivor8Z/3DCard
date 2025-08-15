using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

public abstract class DialogueNodeBase :Node
{
    [Input] public DialogueNodeBase parentNode;

    protected override void Init()
    {
        base.Init();

    }

    public override object GetValue(NodePort port)
    {
        if (port.IsOutput)
        {
            return this;
        }
        else
        {
            return null;
        }
    }

    


    protected bool isEntered = false; 
    public virtual void OnEnter(){ }
    public abstract E_DialogueNodeStaus Evaluate(Dialogueable theDialogueable);

    public virtual void OnExit() { }

    protected bool isPause = false;
    /// <summary>
    /// 递归暂停子节点
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// 递归播放子节点
    /// </summary>
    public abstract void Play();

    /// <summary>
    /// 递归停止子节点
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// 只是重置当前节点，不会影响子节点的状态
    /// 兼顾初始化功能
    /// </summary>
    public abstract void ResetNode(Dialogueable dialogueable);
}