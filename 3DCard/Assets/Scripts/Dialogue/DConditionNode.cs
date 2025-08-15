using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "DialogueConditionNode", menuName = "Dialogue/DialogueConditionNode")]
public class DConditionNode : DialogueNodeBase
{
    public string conditionCheckName;

    [Output] public DialogueNodeBase nextNode;
    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        base.OnCreateConnection(from, to);

        // 只处理 nextNode 端口的连接
        if (from.fieldName == "nextNode" && to.node is DialogueNodeBase child&&child!=this)
        {
            childNode = child; // 直接设置 childNode
        }
    }

    public override void OnRemoveConnection(NodePort port)
    {
        base.OnRemoveConnection(port);

        // 只处理 nextNode 端口的断开
        if (port.fieldName == "nextNode")
        {
            // 清除 childNode 引用
            childNode = null;
        }
    }

    public void ReflectionAssignment(Dialogueable dialogueable)
    {
        if (string.IsNullOrEmpty(conditionCheckName)) return;
        //通过反射获取Dialogueable中的条件检查方法
        var method = dialogueable.GetType().GetMethod(conditionCheckName);
        if (method != null)
        {
            //创建一个Func<bool>委托来调用该方法
            conditionCheck = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), dialogueable, method);
            Debug.Log($"Method {conditionCheckName} found and assigned in {dialogueable.GetType().Name}");
        }
        else
        {
            Debug.LogError($"Method {conditionCheckName} not found in {dialogueable.GetType().Name}");
        }
    }
    public Func<bool> conditionCheck;

    [SerializeField] public DialogueNodeBase childNode;

    public override E_DialogueNodeStaus Evaluate(Dialogueable theDialogueable)
    {
        if (isPause) return E_DialogueNodeStaus.E_Running;
        if (conditionCheck())
        {
            return childNode.Evaluate(theDialogueable);
        }
        else
            return E_DialogueNodeStaus.E_Failure;
    }

    public override void Pause()
    {
        isPause = true;
        if (childNode != null)
            childNode.Pause();
    }

    public override void Play()
    {
        isPause = false;
        if (childNode != null)
            childNode.Play();
    }

    public override void ResetNode(Dialogueable dialogueable)
    {

    }

    public override void Stop()
    {

    }
}
