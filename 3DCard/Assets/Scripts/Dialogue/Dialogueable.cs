using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogueable : MonoBehaviour
{
    public DialogueNodeBase rootNode;
    public DialogueNodeBase currentNode;
    public List<DialogueNodeBase> allNodes = new List<DialogueNodeBase>(); // 树中所有的节点


    #region 生命周期函数
    private void Awake()
    {
        
    }
    private void Start()
    {
        Init();
        //反射初始化
        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i] is DConditionNode conditionNode)
            {
                conditionNode.ReflectionAssignment(this);
            }
            else if (allNodes[i] is DActionNode actionNode)
            {
                actionNode.ReflectionAssignment(this);
            }
        }
    }
    private void OnEnable()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ifWaitForLong = !ifWaitForLong;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ifClickPlayToBoss = !ifClickPlayToBoss;
        }
    }
    private void OnDisable()
    {
        
    }
    #endregion
    public void Init()
    {
        AddNode(rootNode);
        ResetDialogue();
    }
    private void AddNode(DialogueNodeBase currentNode)
    {
        if (currentNode is DSequenceNode sequenceNode)
        {
            allNodes.Add(sequenceNode);
            for (int i = 0; i < sequenceNode.childrenNodes.Count; i++)
            {
                AddNode(sequenceNode.childrenNodes[i]);
            }
        }
        else if (currentNode is DSelectorNode selectorNode)
        {
            allNodes.Add(selectorNode);
            for (int i = 0; i < selectorNode.childrenNodes.Count; i++)
            {
                AddNode(selectorNode.childrenNodes[i]);
            }
        }
        else if (currentNode is DConditionNode conditionNode) 
        {
            allNodes.Add(conditionNode);
            AddNode(conditionNode.childNode);
        } else if (currentNode is DActionNode actionNode)
        {
            allNodes.Add(actionNode);
        }
    }


    public void ResetDialogue()
    {
        currentNode = rootNode;
        for(int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].ResetNode(this);
        }   

    }

    Coroutine dialogueCoroutine;
    public void StartDialogue()
    {
        dialogueCoroutine = StartCoroutine(OnUpdate());
    }
    public void StopDialogue()
    {
        if (dialogueCoroutine == null) return;
        currentNode.Stop();
        StopCoroutine(dialogueCoroutine);
    }

    public IEnumerator OnUpdate()
    {
        while (true)
        {
            currentNode.Evaluate(this);

            yield return null;
        }
    }
    #region ConditionCheckFunc
    public bool IfClick()
    {
        if (Input.GetMouseButtonDown(0))
            return true;
        return false;
    }
    private bool ifClickPlayToBoss = false;
    public bool IfDragPlayToBoss()
    {

        return ifClickPlayToBoss;
    }


    private bool ifWaitForLong = false;
    public bool IfWaitForLong()
    {
        
        return ifWaitForLong;
    }
    #endregion
}
