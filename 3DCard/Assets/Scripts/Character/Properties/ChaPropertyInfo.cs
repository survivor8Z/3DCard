using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 适应因为enemy不能继承Cha,所以做成组件
/// </summary>
public class ChaPropertyInfo:MonoBehaviour
{
    public ChaPropertyData chaData;
    public int curHp;//当前血量
    public int curStr;//当前力量
    public int curDef;//当前防御
    public int curDex;//当前敏捷
    public int curArmor;//当前护甲

    public void Init()
    {
        curHp = chaData.maxHp;
        curStr = chaData.defaultStr;
        curDef = chaData.defaultDef;
        curDex = chaData.defaultDex;
        curArmor = chaData.defaultArmor;
    }
}

