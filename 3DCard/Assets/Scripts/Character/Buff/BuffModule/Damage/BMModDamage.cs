using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// buffCallBackType一定是Damage相关的回调
/// customInfo 一定是DamageInfo
/// </summary>
[CreateAssetMenu(fileName = "BMModDamage",menuName = "Buff/Module/BMModDamage")]
public class BMModDamage : BuffModuleBase
{
    //这两个属性最好分开给到buff,好做优先级,如果不分开就是先加减再乘除
    public int modValue;// 直接增加多少点伤害,或者说减少多少点伤害,负数就是减少
    public float modPercent;// 多增加百分之多少的伤害,或者说说减少百分之多少的伤害,负数就是减少
    public override void Apply(BuffInfo buffInfo, params object[] customInfo)
    {
        if (customInfo[0] is DamageInfo damageInfo)
        {
            damageInfo.damageValue += modValue;
            damageInfo.damageValue = Mathf.CeilToInt(damageInfo.damageValue * (1 + modPercent));
        }
        else
        {
            Debug.LogError("BMModDamage BuffModule传入的参数错误");
        }

    }
}
