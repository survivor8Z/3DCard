using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "DialogueSequenceNode", menuName = "Dialogue/DialogueSequenceNode")]
public class DSequenceNode : DialogueNodeBase
{
    int index = 0; // ��ǰִ�е��ӽڵ�����
    [SerializeField] public List<DialogueNodeBase> childrenNodes = new List<DialogueNodeBase>();
    [Output] public DialogueNodeBase nextNode;
    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        base.OnCreateConnection(from, to);

        // ֻ���� nextNode �˿ڵ�����
        if (from.fieldName == "nextNode" && to.node is DialogueNodeBase child && child != this)
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

        // ֻ���� nextNode �˿ڵĶϿ�
        if (port.fieldName == "nextNode")
        {
            // ��ȡ���������ӵĽڵ�
            var connectedNodes = new List<DialogueNodeBase>();
            foreach (var connection in port.GetConnections())
            {
                if (connection.node is DialogueNodeBase child)
                {
                    connectedNodes.Add(child);
                }
            }
            // �Ƴ��������ӵĽڵ�
            childrenNodes.RemoveAll(child => !connectedNodes.Contains(child));
        }
    }


    public override E_DialogueNodeStaus Evaluate(Dialogueable theDialogueable)
    {
        if (isPause) return E_DialogueNodeStaus.E_Running;

        E_DialogueNodeStaus staus = childrenNodes[index].Evaluate(theDialogueable);
        switch (staus)
        {
            case E_DialogueNodeStaus.E_Running:
                return E_DialogueNodeStaus.E_Running;
            case E_DialogueNodeStaus.E_Success:
                if (index == childrenNodes.Count - 1)
                    return E_DialogueNodeStaus.E_Success;
                else
                {
                    index++;
                    return E_DialogueNodeStaus.E_Running;
                }
            case E_DialogueNodeStaus.E_Failure:
                return E_DialogueNodeStaus.E_Failure;
            default:
                return E_DialogueNodeStaus.E_Running;
        }
    }

    public override void ResetNode(Dialogueable dialogueable)
    {
        index = 0;
        isPause = false;
    }

    public override void Stop()
    {
        index = 0;
        foreach (var node in childrenNodes)
        {
            node.Stop();
        }
    }

    public override void Pause()
    {
        isPause = true;
        if (index < childrenNodes.Count)
            childrenNodes[index].Pause();

    }

    public override void Play()
    {
        isPause = false;
        if (index < childrenNodes.Count)
            childrenNodes[index].Play();
    }
}
