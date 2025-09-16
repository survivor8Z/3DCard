using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaPropertyData",menuName ="Character/ChaData")]
public class ChaPropertyData :ScriptableObject
{
    public int maxHp;
    public int defaultStr;//初始力量
    public int defaultDef;//初始防御,减少任何伤害
    public int defaultDex;//初始敏捷
    public int defaultArmor;//初始护甲,护甲无法减少穿刺伤害
}
