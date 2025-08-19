using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueActionNode", menuName = "Dialogue/DialogueActionNode")]

public class DActionNode : DialogueNodeBase
{


    public string dialogueText;
    public E_DialogueActionExitType exitType;
    public string audioClipName;

    [HideInInspector] public float waitTime = 2f;
    private float enterTime = 0f;

    private Func<bool> conditionCheck;
    public string conditionCheckName;
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
    public override void OnEnter()
    {
        enterTime = 0f;
        HidePreviousDialogue();
        ShowDialogue();
    }
    public override E_DialogueNodeStaus Evaluate(Dialogueable theDialogueable)
    {
        if (!isEntered)
        {
            OnEnter();
            isEntered = true;
        }

        switch (exitType)
        {
            case E_DialogueActionExitType.E_Wait:
                enterTime += Time.deltaTime;
                if (enterTime >= waitTime)
                {
                    HidePreviousDialogue();
                    return E_DialogueNodeStaus.E_Success;
                }
                return E_DialogueNodeStaus.E_Running;
            case E_DialogueActionExitType.E_Click:
                if (Input.GetMouseButtonDown(0))
                {
                    HidePreviousDialogue();
                    return E_DialogueNodeStaus.E_Success;
                }
                return E_DialogueNodeStaus.E_Running;
            case E_DialogueActionExitType.E_Condition:
                if (conditionCheck())
                {
                    HidePreviousDialogue();
                    return E_DialogueNodeStaus.E_Success;
                }
                return E_DialogueNodeStaus.E_Running;
            case E_DialogueActionExitType.E_DontExit:
                return E_DialogueNodeStaus.E_Running;
            default:
                return E_DialogueNodeStaus.E_Running;
        }

    }
    public override void OnExit()
    {

    }

    public override void Pause()
    {

    }
    public override void Play()
    {

    }
    public override void Stop()
    {

    }

    public override void ResetNode(Dialogueable dialogueable)
    {
        isPause = false;
        isEntered = false;
    }


    public void ShowDialogue()
    {
        UIMgr.Instance.ShowPanel<DialoguePanel>(E_UILayer.Middle, (panel) =>
        {
            panel.GetDialogueData(new(dialogueText, audioClipName));
        });
    }

    public void HidePreviousDialogue()
    {
        UIMgr.Instance.HidePanel<DialoguePanel>();
    }


}
