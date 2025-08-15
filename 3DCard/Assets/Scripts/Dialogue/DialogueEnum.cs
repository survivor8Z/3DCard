using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//配置相关
public enum E_DialogueConditionType
{
    E_Wait,
    E_WaitUntil,
    E_Click,
    E_Condition,
}
public enum E_DialogueActionExitType
{
    E_Wait,
    E_Click,
    E_DontExit,//一直显示
    E_Condition,//需要玩家做出特定动作退出
}

public enum E_ConditionCheckType
{
    E_AlwaysTrue,
    E_AlwaysFalse,
    E_ConditionCheck,
}
//节点类型
public enum E_DialogueNodeType
{
    E_Sequence,
    E_Selector,
    E_Condition,
    E_Action,
}

//节点状态
public enum E_DialogueNodeStaus
{
    E_Running,
    E_Success,
    E_Failure,
}
