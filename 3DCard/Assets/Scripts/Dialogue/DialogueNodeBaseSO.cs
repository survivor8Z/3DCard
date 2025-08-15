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
    /// �ݹ���ͣ�ӽڵ�
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// �ݹ鲥���ӽڵ�
    /// </summary>
    public abstract void Play();

    /// <summary>
    /// �ݹ�ֹͣ�ӽڵ�
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// ֻ�����õ�ǰ�ڵ㣬����Ӱ���ӽڵ��״̬
    /// ��˳�ʼ������
    /// </summary>
    public abstract void ResetNode(Dialogueable dialogueable);
}