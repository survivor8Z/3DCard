using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "DialogueSelectorNode", menuName = "Dialogue/DialogueSelectorNode")]
public class DSelectorNode : DialogueNodeBase
{
    // 子节点列表
    [SerializeField] public List<DialogueNodeBase> childrenNodes = new List<DialogueNodeBase>();

    [Output] public DialogueNodeBase nextNode;
    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        base.OnCreateConnection(from, to);

        // 只处理 nextNode 端口的连接
        if (from.fieldName == "nextNode" && to.node is DialogueNodeBase child &&child!=this)
        {
            if (!childrenNodes.Contains(child))
            {
                childrenNodes.Add(child);
            }
        }
    }

    public override void OnRemoveConnection(NodePort port)
    {
        base.OnRemoveConnection(port);

        // 只处理 nextNode 端口的断开
        if (port.fieldName == "nextNode")
        {
            // 获取所有已连接的节点
            var connectedNodes = new List<DialogueNodeBase>();
            foreach (var connection in port.GetConnections())
            {
                if (connection.node is DialogueNodeBase child)
                {
                    connectedNodes.Add(child);
                }
            }
            // 移除不再连接的节点
            childrenNodes.RemoveAll(child => !connectedNodes.Contains(child));
        }
    }

    public override E_DialogueNodeStaus Evaluate(Dialogueable theDialogueable)
    {
        // 如果节点被暂停，直接返回 Running
        if (isPause)
        {
            return E_DialogueNodeStaus.E_Running;
        }

        // 遍历所有子节点
        for (int i = 0; i < childrenNodes.Count; i++)
        {
            DialogueNodeBase node = childrenNodes[i];

            E_DialogueNodeStaus status = node.Evaluate(theDialogueable);
            
            // 如果任何一个子节点成功，则选择器成功
            if (status == E_DialogueNodeStaus.E_Success)
            {
                return E_DialogueNodeStaus.E_Success;
            }
            // 如果任何一个子节点正在运行，则选择器也处于运行中
            else if (status == E_DialogueNodeStaus.E_Running)
            {
                return E_DialogueNodeStaus.E_Running;
            }
            // 如果子节点失败，则继续下一个循环
        }

        // 只有当所有子节点都失败时，选择器才失败
        return E_DialogueNodeStaus.E_Failure;
    }

    public override void Pause()
    {
        isPause = true;
        // 遍历所有子节点并暂停
        for (int i = 0; i < childrenNodes.Count; i++)
        {
            childrenNodes[i].Pause();
        }
    }

    public override void Play()
    {
        isPause = false;
        // 遍历所有子节点并播放
        for (int i = 0; i < childrenNodes.Count; i++)
        {
            childrenNodes[i].Play();
        }
    }

    public override void ResetNode(Dialogueable dialogueable)
    {
        // 遍历所有子节点并重置
        for (int i = 0; i < childrenNodes.Count; i++)
        {
            childrenNodes[i].ResetNode(dialogueable);
        }
    }

    public override void Stop()
    {
        // 遍历所有子节点并停止
        for (int i = 0; i < childrenNodes.Count; i++)
        {
            childrenNodes[i].Stop();
        }
    }
}